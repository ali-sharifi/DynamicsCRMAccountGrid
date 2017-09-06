using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicsCRMAccountGrid.Domain
{
    public class Account : ObservableObject, ISequencedObject
    {
        #region Fields
        private string name { get; set; }
        public string address1_city { get; set; }
        public string primarycontact { get; set; }
        public string telephone1 { get; set; }
        public string emailaddress1 { get; set; }

        private int p_SequenceNumber;
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Account()
        {
        }

        /// <summary>
        /// Paramterized constructor.
        /// </summary>
        /// <param name="itemName">The name of the account item.</param>
        public Account(string itemName)
        {
            name = itemName;
        }

        /// <summary>
        /// Paramterized constructor.
        /// </summary>
        /// <param name="itemName">The name of the account item.</param>
        /// <param name="itemIndex">The sequential position of the item in a account list.</param>
        public Account(string itemName, int itemIndex)
        {
            name = itemName;
            p_SequenceNumber = itemIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The sequential position of this item in a list of items.
        /// </summary>
        public int SequenceNumber
        {
            get { return p_SequenceNumber; }

            set
            {
                p_SequenceNumber = value;
                base.RaisePropertyChangedEvent("SequenceNumber");
            }
        }

        /// <summary>
        /// The name of the account item.
        /// </summary>
        public string Name
        {
            get { return name; }

            set
            {
                name = value;
                base.RaisePropertyChangedEvent("Text");
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Sets the item name as its ToString() value.
        /// </summary>
        /// <returns>The name of the item.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
