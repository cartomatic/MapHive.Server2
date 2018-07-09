using System;
using System.Collections.Generic;
using System.Text;
using Cartomatic.Utils.JsonSerializableObjects;

namespace MapHive.Core.DataModel
{
    public class SerializableListOfPrivilege : SerializableList<Privilege>
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
