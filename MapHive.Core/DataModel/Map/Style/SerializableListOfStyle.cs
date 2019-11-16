namespace MapHive.Core.DataModel.Map
{
    public class SerializableListOfStyle : Cartomatic.Utils.JsonSerializableObjects.SerializableList<Style>
    {
        /// <summary>
        /// hide the base List Capacity property so EF does not force pushes it into db model!
        /// </summary>
        private new int Capacity
        {
            get => base.Capacity;
            set => base.Capacity = value;
        }
    }
}
