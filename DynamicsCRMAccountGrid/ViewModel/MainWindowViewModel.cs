using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicsCRMAccountGrid.ViewModel.Services;
using DynamicsCRMAccountGrid.ViewModel.Commands;
using DynamicsCRMAccountGrid.Domain;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;

namespace DynamicsCRMAccountGrid
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        // Property variables
        private ObservableCollection<Account> p_AccountList;
        private int p_ItemCount;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            this.Initialize();
        }

        #endregion

        #region Command Properties

        /// <summary>
        /// Deletes the currently-selected item from the account List.
        /// </summary>
        public ICommand DeleteItem { get; set; }

        #endregion

        #region Data Properties

        /// <summary>
        /// A account list.
        /// </summary>
        public ObservableCollection<Account> AccountList
        {
            get { return p_AccountList; }

            set
            {
                p_AccountList = value;
                base.RaisePropertyChangedEvent("AccountList");
            }
        }

        /// <summary>
        /// The currently-selected account item.
        /// </summary>
        public Account SelectedItem { get; set; }

        /// <summary>
        /// The number of items in the account list.
        /// </summary>

        public int ItemCount
        {
            get { return p_ItemCount; }

            set
            {
                p_ItemCount = value;
                base.RaisePropertyChangedEvent("ItemCount");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the ItemCount Property when the AccountList collection changes.
        /// </summary>
        void OnAccountListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.AccountList.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.AccountList);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes this application.
        /// </summary>
        private void Initialize()
        {
            // Initialize commands
            this.DeleteItem = new DeleteItemCommand(this);

            // Create account list
            p_AccountList = new ObservableCollection<Account>();

            // Subscribe to CollectionChanged event
            p_AccountList.CollectionChanged += OnAccountListChanged;

            // Add items to the list
            IOrganizationService service = ConnectToCRM();
            if (service != null)
            {
                p_AccountList = FetchXMLSample(service);
            }

            // Initialize list index
            this.AccountList = SequencingService.SetCollectionSequence(this.AccountList);

            // Update bindings
            base.RaisePropertyChangedEvent("AccountList");
        }

        private static IOrganizationService ConnectToCRM()
        {
            #region connect to D365 online

            var appSettings = ConfigurationManager.AppSettings;
            if (appSettings["discoveryService"] == null || appSettings["orgService"] == null)
            {
                Connection.Result = false;
                return null;
            }


            Uri dInfo = new Uri(appSettings["discoveryService"]);
            ClientCredentials clientcred = new ClientCredentials();
            if (Connection.UserName == null || Connection.Password == null)
            {
                return null;
            }
            clientcred.UserName.UserName = Connection.UserName;
            clientcred.UserName.Password = Connection.Password;
            DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(dInfo, null, clientcred, null);
            try
            {
                dsp.Authenticate();
                Connection.Result = true;
            }
            catch (Exception)
            {
                Connection.Result = false;
                return null;
            }


            RetrieveOrganizationsRequest rosreq = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse retieveOrg = (RetrieveOrganizationsResponse)dsp.Execute(rosreq);

            //get the OrganizationService
            OrganizationServiceProxy _serviceproxy = new OrganizationServiceProxy(new Uri(appSettings["orgService"]), null, clientcred, null);
            _serviceproxy.ServiceConfiguration.CurrentServiceEndpoint.EndpointBehaviors.Add(new ProxyTypesBehavior());
            _serviceproxy.EnableProxyTypes();
            IOrganizationService service = (IOrganizationService)_serviceproxy;

            foreach (OrganizationDetail o in retieveOrg.Details)
            {
                //lblOrg.Text = o.FriendlyName;
                //WhoAmIResponse whoAMIResponse = (WhoAmIResponse)service.Execute(new Microsoft.Crm.Sdk.Messages.WhoAmIRequest());
                //lblUser.Text = whoAMIResponse.UserId.ToString();
                break;
            }

            #endregion
            return service;
        }


        private static ObservableCollection<Account> FetchXMLSample(IOrganizationService service)
        {
            string fetchXml = @"<fetch mapping='logical' output-format='xml-platform' version='1.0' distinct='false'>"
                                  + "<entity name='account'>"
                                    + "<attribute name='name' />"
                                    + "<attribute name='address1_city' />"
                                    + "<attribute name='primarycontactid' />"
                                    + "<attribute name='telephone1' />"
                                    + "<attribute name='accountid' />"
                                    + "<attribute name='emailaddress1' />"
                                    + "<order descending='false' attribute='name' />"
                                    + "<filter type='and'>"
                                      + "<condition attribute='ownerid' operator='eq-userid' />"
                                      + "<condition value='0' attribute='statecode' operator='eq' />"
                                    + "</filter>"
                                    + "<link-entity name='contact' visible='false' link-type='outer' to='primarycontactid' from='contactid' alias='accountprimarycontactidcontactcontactid'>"
                                      + "<attribute name='emailaddress1' />"
                                    + "</link-entity>"
                                 + "</entity>"
                                + "</fetch>";

            var myActiveAccounts = service.RetrieveMultiple(new FetchExpression(fetchXml));

            var accountList = new List<Account>();
            foreach (var item in myActiveAccounts.Entities)
            {
                var account = new Account()
                {
                    Name = item.Attributes.Contains("name") == false ? string.Empty : item.Attributes["name"].ToString(),
                    telephone1 = item.Attributes.Contains("telephone1") == false ? string.Empty : item.Attributes["telephone1"].ToString(),
                    address1_city = item.Attributes.Contains("address1_city") == false ? string.Empty : item.Attributes["address1_city"].ToString(),
                    emailaddress1 = item.Attributes.Contains("emailaddress1") == false ? string.Empty : item.Attributes["emailaddress1"].ToString(),
                    primarycontact = item.Attributes.Contains("primarycontactid") == false ? string.Empty : ((EntityReference)item.Attributes["primarycontactid"]).Name
                };
                accountList.Add(account);

            }
            return new ObservableCollection<Account>(accountList);


        }
        #endregion
    }
}