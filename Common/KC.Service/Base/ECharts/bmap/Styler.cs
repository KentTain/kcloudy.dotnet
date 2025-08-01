///////////////////////////////////////////////////////////
//  Styler.cs
//  Implementation of the Class Styler
//  Generated by Enterprise Architect
//  Created on:      13-2��-2017 0:22:35
//  Original author: Doku
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace KC.Service.Base.ECharts.bmap {
	public class Styler {

		 

		public string color{
			get;
			set;
		}

		public VisibilityType? visibility{
			get;
			set;
		}

		/// 
		/// <param name="color"></param>
		public Styler Color(string color){
		     this.color=color;
		return this; 
		}

		/// 
		/// <param name="visibility"></param>
        public Styler Visibility(VisibilityType visibility)
        {
		     this.visibility=visibility;
		return this; 
		}

	}//end Styler

}//end namespace bmap