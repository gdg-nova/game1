/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.GameState
{
    using System.Collections.Generic;
    using System.Linq;
    using Apex.DataStructures;
    using Apex.Messages;
    using Apex.Services;
    using Apex.Units;

    /// <summary>
    /// Encapsulates all things to do with unit selection.
    /// </summary>
    public sealed class Selections : IHandleMessage<UnitDeathMessage>
    {
        private ICollection<ISelectable> _selected;
        private IList<ISelectable> _selectableUnits;
        private IDictionary<int, ICollection<ISelectable>> _groups;

        /// <summary>
        /// Initializes a new instance of the <see cref="Selections"/> class.
        /// </summary>
        public Selections()
        {
            _selected = new HashSet<ISelectable>();
            _selectableUnits = new List<ISelectable>();
            _groups = new Dictionary<int, ICollection<ISelectable>>();

            GameServices.messageBus.Subscribe(this);
        }

        /// <summary>
        /// Gets the currently selected units.
        /// </summary>
        public IEnumerable<ISelectable> selected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Gets the selectable units.
        /// </summary>
        public IEnumerable<ISelectable> selectableUnits
        {
            get { return _selectableUnits; }
        }

        /// <summary>
        /// Registers the a unit as selectable.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void RegisterSelectable(ISelectable unit)
        {
            _selectableUnits.AddUnique(unit);
        }

        /// <summary>
        /// Tentatively selects all selectable units inside the specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="append">if set to <c>true</c> the selection will append to the current selection.</param>
        public void SelectUnitsAsPendingIn(PolygonXZ bounds, bool append)
        {
            foreach (var unit in _selectableUnits)
            {
                bool selectPending = (append && unit.selected) || bounds.Contains(unit.position);
                unit.MarkSelectPending(selectPending);
            }
        }

        /// <summary>
        /// Selects all selectable units inside the specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="append">if set to <c>true</c> the selection will append to the current selection.</param>
        public void SelectUnitsIn(PolygonXZ bounds, bool append)
        {
            var selected = _selectableUnits.Where(u => bounds.Contains(u.position));
            Select(append, selected);
        }

        /// <summary>
        /// Selects the specified units.
        /// </summary>
        /// <param name="append">if set to <c>true</c> the selection will append to the current selection.</param>
        /// <param name="units">The units.</param>
        public void Select(bool append, params ISelectable[] units)
        {
            Select(append, (IEnumerable<ISelectable>)units);
        }

        /// <summary>
        /// Selects the specified units.
        /// </summary>
        /// <param name="append">if set to <c>true</c> the selection will append to the current selection.</param>
        /// <param name="units">The units.</param>
        public void Select(bool append, IEnumerable<ISelectable> units)
        {
            if (!append)
            {
                DeselectAll();
            }

            units.Apply(u => u.selected = true);
            _selected.AddRange(units);
        }

        /// <summary>
        /// Selects the unit at the specified unit index.
        /// </summary>
        /// <param name="unitIndex">Index of the unit.</param>
        /// <param name="toogle">if set to <c>true</c> this will toggle the selection, i.e. if selected it will deselect and vice versa.</param>
        /// <returns>The selected unit</returns>
        public ISelectable Select(int unitIndex, bool toogle)
        {
            if (unitIndex >= _selectableUnits.Count || unitIndex < 0)
            {
                return null;
            }

            var unit = _selectableUnits[unitIndex];

            if (toogle)
            {
                DeselectAll();
                ToggleSelected(unit, true);
            }
            else
            {
                Select(false, unit);
            }

            return unit;
        }

        /// <summary>
        /// Toggles the selected state of a unit. Actually it only toggles when <paramref name="append"/> is true, otherwise it will select the unit while deselecting all others.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="append">if set to <c>true</c> [append].</param>
        public void ToggleSelected(ISelectable unit, bool append)
        {
            //So this works differently depending on the append modifier. If append then the unit is toggled, if not the unit is not toggled but always selected while all others aren't.
            if (!append)
            {
                DeselectAll();

                unit.selected = true;
                _selected.Add(unit);
                return;
            }

            unit.selected = !unit.selected;

            if (unit.selected)
            {
                _selected.Add(unit);
            }
            else
            {
                _selected.Remove(unit);
            }
        }

        /// <summary>
        /// Deselects all currently selected units.
        /// </summary>
        public void DeselectAll()
        {
            _selected.Apply(u => u.selected = false);
            _selected.Clear();
        }

        /// <summary>
        /// Assigns the currently selected units to a group.
        /// </summary>
        /// <param name="groupIndex">Index of the group.</param>
        public void AssignGroup(int groupIndex)
        {
            ICollection<ISelectable> group;
            if (!_groups.TryGetValue(groupIndex, out group))
            {
                group = new HashSet<ISelectable>();
                _groups.Add(groupIndex, group);
            }
            else
            {
                group.Clear();
            }

            group.AddRange(_selected);
        }

        /// <summary>
        /// Selects a group.
        /// </summary>
        /// <param name="groupIndex">Index of the group.</param>
        public void SelectGroup(int groupIndex)
        {
            ICollection<ISelectable> group;
            if (_groups.TryGetValue(groupIndex, out group))
            {
                Select(false, group);
            }
        }

        void IHandleMessage<UnitDeathMessage>.Handle(UnitDeathMessage message)
        {
            var unit = message.unit.As<ISelectable>();
            if (unit == null)
            {
                return;
            }

            unit.selected = false;
            _selected.Remove(unit);
            _selectableUnits.Remove(unit);

            //No need to create an enumerator if the groups dict is empty
            if (_groups.Count > 0)
            {
                foreach (var grp in _groups)
                {
                    grp.Value.Remove(unit);
                }
            }
        }
    }
}
