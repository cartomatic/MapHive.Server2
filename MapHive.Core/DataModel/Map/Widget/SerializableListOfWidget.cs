namespace MapHive.Core.DataModel.Map
{
    public class SerializableListOfWidget : Cartomatic.Utils.JsonSerializableObjects.SerializableList<Widget>
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
