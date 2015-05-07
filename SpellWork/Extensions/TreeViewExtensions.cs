using System.Linq;
using System.Windows.Forms;
using SpellWorkLib.Spell;

namespace SpellWork.Extensions
{
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Returns the value of the collection of selected items
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static uint[] GetMask(this TreeView tv)
        {
            var val = new uint[3];
            foreach (var node in tv.Nodes.Cast<TreeNode>().Where(node => node.Checked))
            {
                if (node.Index < 32)
                    val[0] += 1U << node.Index;
                else if(node.Index < 64)
                    val[1] += 1U << (node.Index - 32);
                else
                    val[2] += 1U << (node.Index - 64);
            }
            return val;
        }
    }
}
