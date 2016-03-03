﻿using FactorioCalculator.Models;
using FactorioCalculator.Helper;
using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FactorioCalculator.Importer
{
    class ModImporter
    {
        public string FactorioPath { get; private set; }
        public string ModName { get; private set; }

        public string ModulePath { get { return Path.Combine(FactorioPath, "data", ModName); } }

        public Library Library { get; private set; }

        public bool IsLoaded { get; private set; }

        public ModImporter(string factorioPath, string modName = "base")
        {
            FactorioPath = factorioPath;
            ModName = modName;
        }

        public static DirectoryInfo AppDataFolder { get { return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FactorioCalculator")); } }

        public void Load()
        {
            using(var lua = new Lua())
            {
                var corepath = Path.Combine(FactorioPath, "data", "core");

                var appdata = AppDataFolder; 
                if (!appdata.Exists)
                    appdata.Create();

                CopyDirectory(new DirectoryInfo(Path.Combine(corepath, "lualib")), appdata);
                Directory.SetCurrentDirectory(appdata.FullName);

                //Import global classes
                foreach (var file in new String[] { "util.lua", "defines.lua", "dataloader.lua" })
                {
                    using (var f = lua.LoadFile(file))
                        f.Call(file);
                }

                CopyDirectory(new DirectoryInfo(ModulePath), appdata);

                
                using (var f = lua.LoadFile("data.lua"))
                    f.Call("data.lua");

                LuaTable data = lua["data"] as LuaTable;
                var raw = data["raw"] as LuaTable;
                var resources = raw["resource"] as LuaTable;
                var recipes = raw["recipe"] as LuaTable;

                var library = new Library();

                foreach (var recipe in ParseRecipes(recipes))
                    library.AddRecipe(recipe);

                foreach (var building in ParseEntities(raw))
                    library.AddBuilding(building);

                foreach (var subtree in new string[] { "item", "fluid", "ammo", "module", "mining-tool", "armor", "capsule", "gun", "blueprint", "deconstruction-item", "repair-tool", "tool", "generator", "boiler" })
                    foreach (var item in ParseItems(raw[subtree] as LuaTable))
                        library.AddItem(item);

                foreach(var resource in resources.Values.OfType<LuaTable>())
                {
                    if (!resource.ContainsKey("minable"))
                        continue;
                    var minable = resource["minable"] as LuaTable;
                    
                    if (minable.ContainsKey("results"))
                    {
                        var items = minable["results"] as LuaTable;
                        foreach(var item in items.Values.OfType<LuaTable>())
                            library.Items.Where((i) => i.Name == item["name"] as string).First().IsResource = true;
                    }

                    if(minable.ContainsKey("result"))
                    {
                        var item = minable["result"] as string;
                        library.Items.Where((i) => i.Name == item).First().IsResource = true;
                    }
                }

                Library = library;
            }

            Library.Initialize();
        }

        private IEnumerable<Building> ParseEntities(LuaTable entities)
        {
            foreach(var entitylist in entities.Values.OfType<LuaTable>())
            {
                foreach (var entity in entitylist.Values.OfType<LuaTable>())
                {
                    //HACK: try to find a better way to select only entities
                    if (!entity.ContainsKey("icon") || (string)entity["type"] == "technology")
                        continue;

                    var name = entity["name"] as string;

                    var result = new Building(name);
                    result.IconPath = ((string)entity["icon"]).Replace("__base__", AppDataFolder.FullName);

                    if (entity.ContainsKey("selection_box"))
                    {
                        var selectionBox = entity["selection_box"] as LuaTable;
                        var v1 = (selectionBox[1.0] as LuaTable).ToVector2();
                        var v2 = (selectionBox[2.0] as LuaTable).ToVector2();
                        result.Size = v2 - v1;
                        result.Size = new Vector2(Math.Ceiling(result.Size.X), Math.Ceiling(result.Size.Y));
                    }

                    if (entity.ContainsKey("fluid_boxes"))
                    {
                        var boxes = entity["fluid_boxes"] as LuaTable;
                        
                        foreach (KeyValuePair<Object, Object> pair in boxes)
                        {
                            if (pair.Key.Equals("off_when_no_fluid_recipe"))
                            {
                                result.HidesFluidBox = (bool)pair.Value;
                            }
                            else
                            {
                                var values = pair.Value as LuaTable;
                                var center = (result.Size - Vector2.One) / 2;
                                result.AddFluidBox(new FluidBox(
                                    (values["production_type"] as string).Equals("output", StringComparison.OrdinalIgnoreCase),
                                    (((values["pipe_connections"] as LuaTable)[1.0] as LuaTable)["position"] as LuaTable).ToVector2() + center
                                    ));
                            }
                        }
                    }

                    if (entity.ContainsKey("crafting_categories"))
                    {
                        var categories = entity["crafting_categories"] as LuaTable;
                        foreach (var cat in categories.Values.OfType<string>())
                            result.AddCraftingCategory(cat);
                        if (result.CraftingCategories.Contains("smelting"))
                            result.IngredientCount = 1;
                    }

                    if (entity.ContainsKey("crafting_speed"))
                        result.ProductionSpeed = (double)entity["crafting_speed"];

                    if (entity.ContainsKey("ingredient_count"))
                        result.IngredientCount = (int)((double)entity["ingredient_count"]);

                    if (entity.ContainsKey("energy_source"))
                    {
                        var source = entity["energy_source"] as LuaTable;

                        double efficiency = 1;
                        result.EnergySource = (EnergySource)Enum.Parse(typeof(EnergySource), source["type"] as string, true);

                        if (result.EnergySource == EnergySource.Burner)
                            if (source.ContainsKey("effectivity"))
                                efficiency = (double)source["effectivity"];

                        if (entity.ContainsKey("energy_usage"))
                        {
                            result.Energy = ParseEnergy(entity["energy_usage"] as string) / efficiency;
                        }
                        else if (source.ContainsKey("drain"))
                        {
                            result.Energy = ParseEnergy(source["drain"] as string) / efficiency;
                        }
                        else if(entity.Keys.OfType<string>().Where((s) => s.StartsWith("energy_per_")).Any())
                        {
                            string key = entity.Keys.OfType<string>().Where((s) => s.StartsWith("energy_per_")).First();
                            string energy = entity[key] as string;
                            if (!string.IsNullOrWhiteSpace(energy))
                                result.Energy = ParseEnergy(energy);
                            else
                                result.Energy = (double)entity[key];
                        }
                    }

                    yield return result;
                }
            }
        }

        private IEnumerable<Item> ParseItems(LuaTable items)
        {
            foreach(string name in items.Keys)
            {
                var properties = items[name] as LuaTable;
                var result = new Item(name);

                if (properties.ContainsKey("place_result"))
                    result.PlaceResultName = properties["place_result"] as string;

                if (properties.ContainsKey("heat_capacity"))
                    result.ItemType = ItemType.Fluid;
                else
                    result.ItemType = ItemType.Solid;

                if (properties.ContainsKey("fuel_value"))
                    result.FuelValue = ParseEnergy(properties["fuel_value"] as string);

                yield return result;
            }
        }

        private static IEnumerable<Recipe> ParseRecipes(LuaTable recipes)
        {
            foreach (string name in recipes.Keys)
            {
                var recipe = recipes[name] as LuaTable;
                var result = new Recipe(name);

                var ingredients = recipe["ingredients"] as LuaTable;
                foreach (var ingredient in ParseItemAmounts(ingredients))
                    result.AddIngredient(ingredient);

                if (recipe.ContainsKey("result"))
                {
                    double amount = 1;
                    if (recipe.ContainsKey("result_count"))
                        amount = (double)recipe["result_count"];
                    result.AddResult(new ItemAmount(recipe["result"] as string, amount));
                }
                else
                {
                    var results = recipe["results"] as LuaTable;
                    foreach (var output in ParseItemAmounts(results))
                        result.AddResult(output);
                }

                var category = recipe["category"] as string;
                if (category != null)
                    result.CraftingCategory = category;

                if (recipe.ContainsKey("energy_required"))
                    result.Time = (double)recipe["energy_required"];

                yield return result;
            }
        }

        private static double ParseEnergy(string energy)
        {
            energy = energy.Trim();
            
            double multiplier = 1;
            if (energy.EndsWith("KW", StringComparison.OrdinalIgnoreCase) ||
                energy.EndsWith("KJ", StringComparison.OrdinalIgnoreCase))
                multiplier = 1000;
            if (energy.EndsWith("MW", StringComparison.OrdinalIgnoreCase) ||
                energy.EndsWith("MJ", StringComparison.OrdinalIgnoreCase))
                multiplier = 1000*1000;
            if (energy.EndsWith("GW", StringComparison.OrdinalIgnoreCase) ||
                energy.EndsWith("GJ", StringComparison.OrdinalIgnoreCase))
                multiplier = 1000 * 1000 * 1000;

            Regex filter = new Regex("^[0-9,.-]+");
            var match = filter.Match(energy);
            if (!match.Success)
                throw new FormatException("Energy not in right format");

            var result = Double.Parse(match.Value, CultureInfo.InvariantCulture);
            result *= multiplier;

            return result;
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!source.Exists)
                throw new DirectoryNotFoundException();
            if (!destination.Exists)
                destination.Create();

            foreach(var file in source.EnumerateFiles())
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true);

            foreach (var dir in source.EnumerateDirectories())
                CopyDirectory(dir, new DirectoryInfo(Path.Combine(destination.FullName, dir.Name)));
        }

        private static IEnumerable<ItemAmount> ParseItemAmounts(LuaTable table)
        {
            foreach (LuaTable tuple in table.Values)
            {
                string ingredientName = null;
                double amount = 0;

                foreach (object key in tuple.Keys)
                {
                    string keyName;
                    if (key is double)
                    {
                        switch ((int)((double)key))
                        {
                            case 1:
                                keyName = "name";
                                break;
                            case 2:
                                keyName = "amount";
                                break;
                            default:
                                keyName = "unknown";
                                break;
                        }
                    }
                    else
                    {
                        keyName = key as string;
                    }

                    if (keyName == "name")
                        ingredientName = tuple[key] as string;
                    if (keyName == "amount")
                        amount = (double)tuple[key];
                }
                if (String.IsNullOrWhiteSpace(ingredientName))
                    continue;
                if (amount <= 0)
                    continue;

                var ingredient = new ItemAmount(ingredientName, amount);
                yield return ingredient;
            }
        }
    }
}
