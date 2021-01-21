namespace SirRandoo.ToolkitUtils.Models.Tables
{
    public abstract class TableItem<T>
    {
        public T Data { get; set; }
        public bool IsHidden { get; set; }
    }
}
