namespace NetworkCheck
{
	using System;
	using System.Net.NetworkInformation;
	using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides notification of status changes related to Internet-specific network
    /// adapters on this machine.  All other adpaters such as tunneling and loopbacks
    /// are ignored.  Only connected IP adapters are considered.
    /// </summary>

    public static class NetworkStatus
	{
		private static bool isAvailable;
		private static NetworkStatusChangedHandler handler;

		/// <summary>
		/// Initialize the class by detecting the start condition.
		/// </summary>

		static NetworkStatus ()
		{
			isAvailable = IsNetworkAvailable();
		}

		/// <summary>
		/// This event is fired when the overall Internet connectivity changes.  All
		/// non-Internet adpaters are ignored.  If at least one valid Internet connection
		/// is "up" then we consider the Internet "available".
		/// </summary>

		public static event NetworkStatusChangedHandler AvailabilityChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				if (handler == null)
				{
					NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);
					NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
				}
				handler = (NetworkStatusChangedHandler)Delegate.Combine(handler, value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				handler = (NetworkStatusChangedHandler)Delegate.Remove(handler, value);
				if (handler == null)
				{
					NetworkChange.NetworkAvailabilityChanged -= new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);
					NetworkChange.NetworkAddressChanged -= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
				}
			}
		}

		/// <summary>
		/// Gets a Boolean value indicating the current state of Internet connectivity.
		/// </summary>

		public static bool IsAvailable
		{
			get { return isAvailable; }
		}

		/// <summary>
		/// Evaluate the online network adapters to determine if at least one of them
		/// is capable of connecting to the Internet.
		/// </summary>
		/// <returns></returns>

		private static bool IsNetworkAvailable ()
		{
			if (NetworkInterface.GetIsNetworkAvailable())
			{
				NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface face in interfaces)
				{
					if (face.OperationalStatus == OperationalStatus.Up)
					{
						if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
							(face.NetworkInterfaceType != NetworkInterfaceType.Loopback) && (face.Description.ToLower().StartsWith("vmware") != true))
                        {
							IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();
							if ((statistics.BytesReceived > 0) && (statistics.BytesSent > 0))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private static void DoNetworkAddressChanged (object sender, EventArgs e)
		{
			SignalAvailabilityChange(sender);
		}

		private static void DoNetworkAvailabilityChanged (object sender, NetworkAvailabilityEventArgs e)
		{
			SignalAvailabilityChange(sender);
		}

		private static void SignalAvailabilityChange (object sender)
		{
			bool change = IsNetworkAvailable();
			if (change != isAvailable)
			{
				isAvailable = change;
				if (handler != null)
				{
					handler(sender, new NetworkStatusChangedArgs(isAvailable));
				}
			}
		}
	}
}