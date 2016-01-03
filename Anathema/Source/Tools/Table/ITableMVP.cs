﻿using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anathema
{
    delegate void TableEventHandler(Object Sender, TableEventArgs Args);
    class TableEventArgs : EventArgs
    {
        public Int32 AddressTableItemCount = 0;
        public Int32 ScriptTableItemCount = 0;
        public Int32 FSMTableItemCount = 0;
    }

    interface ITableView : IScannerView
    {
        // Methods invoked by the presenter (upstream)
        void RefreshDisplay();
        void UpdateAddressTableItemCount(Int32 ItemCount);
        void UpdateScriptTableItemCount(Int32 ItemCount);
        void UpdateFSMTableItemCount(Int32 ItemCount);
    }

    abstract class ITableModel : IScannerModel
    {
        // Events triggered by the model (upstream)
        public event TableEventHandler EventRefreshDisplay;
        protected virtual void OnEventRefreshDisplay(TableEventArgs E)
        {
            if (EventRefreshDisplay != null) EventRefreshDisplay(this, E);
        }

        public event TableEventHandler EventUpdateAddressTableItemCount;
        protected virtual void OnEventUpdateAddressTableItemCount(TableEventArgs E)
        {
            EventUpdateAddressTableItemCount(this, E);
        }

        public event TableEventHandler EventUpdateScriptTableItemCount;
        protected virtual void OnEventUpdateScriptTableItemCount(TableEventArgs E)
        {
            EventUpdateScriptTableItemCount(this, E);
        }

        public event TableEventHandler EventUpdateFSMTableItemCount;
        protected virtual void OnEventUpdateFSMTableItemCount(TableEventArgs E)
        {
            EventUpdateFSMTableItemCount(this, E);
        }

        // Functions invoked by presenter (downstream)
        public abstract AddressItem GetAddressItemAt(Int32 Index);
        public abstract void SetAddressItemAt(Int32 Index, AddressItem AddressItem);
        public abstract void SetFrozenAt(Int32 Index, Boolean Activated);

        public abstract ScriptItem GetScriptItemAt(Int32 Index);
        public abstract void SetScriptItemAt(Int32 Index, ScriptItem ScriptItem);
    }

    class TablePresenter : Presenter<ITableView, ITableModel>
    {
        protected new ITableView View { get; set; }
        protected new ITableModel Model { get; set; }

        private Int32 CacheLimit = 2048;
        private Dictionary<Int32, ListViewItem> AddressTableCache;
        private Dictionary<Int32, ListViewItem> ScriptTableCache;

        public TablePresenter(ITableView View, ITableModel Model) : base(View, Model)
        {
            this.View = View;
            this.Model = Model;

            AddressTableCache = new Dictionary<Int32, ListViewItem>();
            ScriptTableCache = new Dictionary<Int32, ListViewItem>();

            // Bind events triggered by the model
            Model.EventRefreshDisplay += EventRefreshDisplay;
            Model.EventUpdateAddressTableItemCount += EventUpdateAddressTableItemCount;
            Model.EventUpdateScriptTableItemCount += EventUpdateScriptTableItemCount;
            Model.EventUpdateFSMTableItemCount += EventUpdateFSMTableItemCount;
        }

        #region Method definitions called by the view (downstream)

        public ListViewItem GetAddressTableItemAt(Int32 Index)
        {
            AddressItem AddressItem = Model.GetAddressItemAt(Index);

            if (AddressTableCache.Count > CacheLimit)
                AddressTableCache.Clear();

            // Insert item into cache if not present
            if (!AddressTableCache.ContainsKey(Index))
                AddressTableCache.Add(Index, new ListViewItem(new String[] { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty }));

            // Update properties
            AddressTableCache[Index].SubItems[0].Text = String.Empty;
            AddressTableCache[Index].SubItems[1].Text = AddressItem.Description.ToString();
            AddressTableCache[Index].SubItems[2].Text = Conversions.ToAddress(AddressItem.Address.ToString());
            AddressTableCache[Index].SubItems[3].Text = AddressItem.ElementType.Name.ToString();
            AddressTableCache[Index].SubItems[4].Text = AddressItem.Value.ToString();
            AddressTableCache[Index].Checked = AddressItem.GetActivationState();

            return AddressTableCache[Index];
        }

        public ListViewItem GetScriptTableItemAt(Int32 Index)
        {
            ScriptItem ScriptItem = Model.GetScriptItemAt(Index);

            if (ScriptTableCache.Count > CacheLimit)
                ScriptTableCache.Clear();

            ListViewItem Result = new ListViewItem(ScriptItem.Description.ToString());
            Result.Checked = ScriptItem.GetActivationState();
            return Result;
        }

        public void SetFrozenAt(Int32 Index, Boolean Activated)
        {
            Model.SetFrozenAt(Index, Activated);
        }

        public String GetScriptTableScriptAt(Int32 Index)
        {
            return Model.GetScriptItemAt(Index).Script;
        }

        public ListViewItem GetFSMTableItemAt(Int32 Index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event definitions for events triggered by the model (upstream)

        private void EventRefreshDisplay(Object Sender, TableEventArgs E)
        {
            View.RefreshDisplay();
        }

        private void EventUpdateAddressTableItemCount(Object Sender, TableEventArgs E)
        {
            View.UpdateAddressTableItemCount(E.AddressTableItemCount);
        }

        private void EventUpdateScriptTableItemCount(Object Sender, TableEventArgs E)
        {
            View.UpdateAddressTableItemCount(E.ScriptTableItemCount);
        }

        private void EventUpdateFSMTableItemCount(Object Sender, TableEventArgs E)
        {
            View.UpdateAddressTableItemCount(E.FSMTableItemCount);
        }

        #endregion
    }
}