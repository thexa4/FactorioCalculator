using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FactorioCalculator.Models;

namespace FactorioCalculator.Forms
{
    public partial class TreeViewer : Form
    {
        private readonly Library _library;

        public TreeViewer(Library library)
        {
            _library = library;


            InitializeComponent();
        }

        private void TreeViewer_Load(object sender, EventArgs e)
        {
            ItemsSelector.Items.Clear();

            var craftable = from item in _library.Items
                            where item.Recipes.Any()
                            orderby item.Name
                            select item;


            foreach (var item in craftable)
                ItemsSelector.Items.Add(item);
        }

        private void TreeViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private async void ItemsSelector_SelectedItemChanged(object sender, EventArgs e)
        {
            await RenderItemCraftingTree((Item)ItemsSelector.SelectedItem);
        }

        private async Task RenderItemCraftingTree(Item item)
        {
            var totals = new ConcurrentDictionary<Item, double>();

            RecipeTreeView.Nodes.Clear();
            RecipeTreeView.Nodes.AddRange(ToTreeNodes(item, 1, totals));
            foreach (var node in RecipeTreeView.Nodes)
                ((TreeNode)node).ExpandAll();

            foreach (var res in totals)
                TotalsDisplay.Items.Add(string.Format("{0} ({1})", res.Key.Name, res.Value));
        }

        private IEnumerable<TreeNode> ToTreeNodes(Recipe recipe, double amount, ConcurrentDictionary<Item, double> totals)
        {
            var node = new TreeNode(string.Format("{0} ({1})", recipe.Name, amount));

            foreach (var ingredient in recipe.Ingredients)
                node.Nodes.AddRange(ToTreeNodes(ingredient.Item, ingredient.Amount * amount, totals));

            return new[] { node };
        }

        private TreeNode[] ToTreeNodes(Item item, double amount, ConcurrentDictionary<Item, double> totals)
        {
            if (item.IsResource)
            {
                totals.AddOrUpdate(item, amount, (_, b) => b + amount);

                return new[] {
                    new TreeNode(string.Format("{0} - ({1})", item.Name, amount)),
                };
            }
            else
            {
                return (from recipe in item.Recipes
                        let amt = recipe.Results.Single(a => a.Item == item).Amount
                        from node in ToTreeNodes(recipe, amount / amt, totals)
                        select node).ToArray();
            }
        }
    }
}
