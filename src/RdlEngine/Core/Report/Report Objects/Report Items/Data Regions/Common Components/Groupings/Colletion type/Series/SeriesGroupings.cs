/*
 * This file is part of UniERM ReportDesigner, based on reportFU by Josh Wilson,
 * the work of Kim Sheffield and the fyiReporting project. 
 * 
 * Prior Copyrights:
 * _________________________________________________________
 * |Copyright (C) 2010 devFU Pty Ltd, Josh Wilson and Others|
 * | (http://reportfu.org)                                  |
 * =========================================================
 * _________________________________________________________
 * |Copyright (C) 2004-2008  fyiReporting Software, LLC     |
 * |For additional information, email info@fyireporting.com |
 * |or visit the website www.fyiReporting.com.              |
 * =========================================================
 *
 * License:
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Reporting.Rdl
{
	///<summary>
	/// Collection of Chart series groupings.
	///</summary>
	[Serializable]
	internal class SeriesGroupings : ReportLink
	{
        List<SeriesGrouping> _Items;			// list of SeriesGrouping

		internal SeriesGroupings(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			SeriesGrouping sg;
            _Items = new List<SeriesGrouping>();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "SeriesGrouping":
						sg = new SeriesGrouping(r, this, xNodeLoop);
						break;
					default:
						sg=null;		// don't know what this is
						break;
				}
				if (sg != null)
					_Items.Add(sg);
			}
			if (_Items.Count == 0)
				OwnerReport.rl.LogError(8, "For SeriesGroupings at least one SeriesGrouping is required.");
			else
                _Items.TrimExcess();
		}
		
		override internal void FinalPass()
		{
			foreach (SeriesGrouping sg in _Items)
			{
				sg.FinalPass();
			}
			return;
		}

        internal List<SeriesGrouping> Items
		{
			get { return  _Items; }
		}
	}

    ///<summary>
    /// Chart Series grouping (both dynamic and static).
    ///</summary>
    [Serializable]
    internal class SeriesGrouping : ReportLink
    {
        DynamicSeries _DynamicSeries;	// Dynamic Series headings for this grouping
        StaticSeries _StaticSeries;		// Static Series headings for this grouping	
        Style _Style;					// border and background properties for series legend itmes and data points
        //   when dynamic exprs are evaluated per group instance

        internal SeriesGrouping(ReportDefn r, ReportLink p, XmlNode xNode)
            : base(r, p)
        {
            _DynamicSeries = null;
            _StaticSeries = null;
            _Style = null;

            // Loop thru all the child nodes
            foreach (XmlNode xNodeLoop in xNode.ChildNodes)
            {
                if (xNodeLoop.NodeType != XmlNodeType.Element)
                    continue;
                switch (xNodeLoop.Name)
                {
                    case "DynamicSeries":
                        _DynamicSeries = new DynamicSeries(r, this, xNodeLoop);
                        break;
                    case "StaticSeries":
                        _StaticSeries = new StaticSeries(r, this, xNodeLoop);
                        break;
                    case "Style":
                        _Style = new Style(OwnerReport, this, xNodeLoop);
                        OwnerReport.rl.LogError(4, "Style element in SeriesGrouping is currently ignored."); // TODO
                        break;
                    default:
                        // don't know this element - log it
                        OwnerReport.rl.LogError(4, "Unknown SeriesGrouping element '" + xNodeLoop.Name + "' ignored.");
                        break;
                }
            }
        }

        override internal void FinalPass()
        {
            if (_DynamicSeries != null)
                _DynamicSeries.FinalPass();
            if (_StaticSeries != null)
                _StaticSeries.FinalPass();
            if (_Style != null)
                _Style.FinalPass();

            return;
        }

        internal DynamicSeries DynamicSeries
        {
            get { return _DynamicSeries; }
            set { _DynamicSeries = value; }
        }

        internal StaticSeries StaticSeries
        {
            get { return _StaticSeries; }
            set { _StaticSeries = value; }
        }

        internal Style Style
        {
            get { return _Style; }
            set { _Style = value; }
        }

    }
}
