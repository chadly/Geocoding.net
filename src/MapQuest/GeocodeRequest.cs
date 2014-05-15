﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	public class GeocodeRequest : ReverseGeocodeRequest
	{
		public GeocodeRequest(string key, string address)
			: this(key, new LocationRequest(address))
		{
		}

		public GeocodeRequest(string key, LocationRequest loc) 
			: base(key, loc)
		{
		}

		public override string RequestAction
		{
			get { return "address"; }
		}
	}
}
