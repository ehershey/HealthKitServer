﻿using System;
using MonoTouch.HealthKit;
using MonoTouch.Foundation;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace HealthKitServer
{
	public class HealtKitAccess : IHealthKitAccess
	{
		private HKHealthStore m_healthKitStore;


		public void SetUpPermissions ()
		{
			var distanceQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.DistanceWalkingRunning);
			var stepsQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var flightsQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.FlightsClimbed);
			var heightQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.Height);
			var heartRateQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.HeartRate);
			var nikeFuelQuantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.NikeFuel);
			var dateOfBirthCharacteristicType = HKObjectType.GetCharacteristicType (HKCharacteristicTypeIdentifierKey.DateOfBirth);
			var sexCharacteristicType = HKObjectType.GetCharacteristicType (HKCharacteristicTypeIdentifierKey.BiologicalSex);
			var bloodTypeCharacteristicType = HKObjectType.GetCharacteristicType (HKCharacteristicTypeIdentifierKey.BloodType);

			if (m_healthKitStore == null)
			{
				HealthKitStore = new HKHealthStore ();
				m_healthKitStore.RequestAuthorizationToShare (new NSSet (new [] { distanceQuantityType , stepsQuantityType , flightsQuantityType  }), new NSSet (new [] {  (NSObject) distanceQuantityType ,(NSObject)  stepsQuantityType , (NSObject) flightsQuantityType , (NSObject)  heightQuantityType , (NSObject)dateOfBirthCharacteristicType, (NSObject) sexCharacteristicType, (NSObject) bloodTypeCharacteristicType, (NSObject)nikeFuelQuantityType, (NSObject)bloodTypeCharacteristicType  }), (success, error) => {
					Console.WriteLine ("Authorized:" + success);
					if (error != null) {
						Console.WriteLine ("Authorization error: " + error);
					}
				});

			}
		}

		public HKHealthStore HealthKitStore
		{
			get
			{
				return m_healthKitStore;
			}
			private set
			{
				m_healthKitStore = value;
			}
		}

		public async Task<string>  QueryTotalSteps()
		{
			var stepsCount = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var sumOptions = HKStatisticsOptions.CumulativeSum;
			string resultString = string.Empty;
			var query = new HKStatisticsQuery(stepsCount, new NSPredicate (IntPtr.Zero), sumOptions, (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {
				if (results != null) {
					var quantitySample = results;
					var quantity =  quantitySample.SumQuantity();
				//	resultString = quantity.ToString();
					HealthKitDataContext.ActiveHealthKitData.DistanceReadings.TotalSteps = quantity.ToString();
					Console.WriteLine(string.Format("totally walked {0} steps",quantity.ToString()));
				}

			});
			await Task.Factory.StartNew(() =>HealthKitStore.ExecuteQuery (query));
			return resultString;
		}

		public async Task<string>  QueryTotalStepsRecordingFirstRecordingDate()
		{
			var stepsCount = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var sumOptions = HKStatisticsOptions.CumulativeSum;
			string resultString = string.Empty;
			var query = new HKStatisticsQuery(stepsCount, new NSPredicate (IntPtr.Zero), sumOptions, (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {
				if (results != null) {
					var quantitySample = results;
					var quantity =  quantitySample.StartDate;
					//	resultString = quantity.ToString();
					HealthKitDataContext.ActiveHealthKitData.DistanceReadings.RecordingStarted = quantity.ToString();
					Console.WriteLine(string.Format("Started recording steps: {0} ",quantity.ToString()));
				}

			});
			await Task.Factory.StartNew(() =>HealthKitStore.ExecuteQuery (query));
			return resultString;
		}

		public async Task<string>  QueryTotalStepsRecordingLastRecordingDate()
		{
			var stepsCount = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var sumOptions = HKStatisticsOptions.CumulativeSum;
			string resultString = string.Empty;
			var query = new HKStatisticsQuery(stepsCount, new NSPredicate (IntPtr.Zero), sumOptions, (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {
				if (results != null) {
					var quantitySample = results;
					var quantity =  quantitySample.EndDate;
					//	resultString = quantity.ToString();
					HealthKitDataContext.ActiveHealthKitData.DistanceReadings.RecordingStoped = quantity.ToString();
					Console.WriteLine(string.Format("Last recording of steps: {0} ",quantity.ToString()));
				}

			});
			await Task.Factory.StartNew(() =>HealthKitStore.ExecuteQuery (query));
			return resultString;
		}

		public async Task<string> QueryTotalLengthWalked()
		{
			var stepsCount = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.DistanceWalkingRunning);
			var sumOptions = HKStatisticsOptions.CumulativeSum;
			string resultString = string.Empty;
			var query = new HKStatisticsQuery(stepsCount, new NSPredicate (IntPtr.Zero), sumOptions,(HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {
				if (results != null) {
					var quantitySample = results;
					var quantity = quantitySample.SumQuantity();
					// resultString = quantity.ToString();;
					HealthKitDataContext.ActiveHealthKitData.DistanceReadings.TotalDistance = quantity.ToString();
					Console.WriteLine(string.Format("totally walked {0}",quantity.ToString()));

				}

			});
			await Task.Factory.StartNew(() => HealthKitStore.ExecuteQuery (query));
			return resultString;
		}

//		public async Task<double> QueryTotalHeight()
//		{
//
//			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height);
//			double usersHeight = 0.0;
//			FetchMostRecentData (heightType, (mostRecentQuantity, error) => {
//				if (error != null) {
//					Console.WriteLine ("An error occured fetching the user's height information. " +
//						"In your app, try to handle this gracefully. The error was: {0}.", error.LocalizedDescription);
//					return;
//				}
//
//				if (mostRecentQuantity != null) {
//					var heightUnit = HKUnit.Meter;
//					usersHeight = mostRecentQuantity.GetDoubleValue (heightUnit);
//					HealthKitDataContext.ActiveHealthKitData.Height = usersHeight;
//
//				}
//
//			});
//			Console.WriteLine(string.Format("Total height: ", usersHeight));
//			return usersHeight;
//		}

		public async Task<double> QueryTotalHeight()
		{

			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height);
			double usersHeight = 0.0;

			var timeSortDescriptor = new NSSortDescriptor (HKSample.SortIdentifierEndDate, false);
			var query = new HKSampleQuery (heightType, new NSPredicate (IntPtr.Zero), 1, new NSSortDescriptor[] { timeSortDescriptor },
				(HKSampleQuery resultQuery, HKSample[] results, NSError error) => {

					HKQuantity quantity = null;
					string resultString = string.Empty;
					if (results.Length != 0) {
						resultString = results [results.Length - 1].ToString();
						HealthKitDataContext.ActiveHealthKitData.Height = ParseStringResultToDuble(resultString);
						Console.WriteLine(string.Format("value of Fetched: {0}", ParseStringResultToDuble(resultString)));
					}

				});
			m_healthKitStore.ExecuteQuery (query);
			Console.WriteLine(string.Format("Total height: ", usersHeight));
			return usersHeight;
		}


		public async Task<double> QueryLastRegistratedWalkingDistance()
		{

			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.DistanceWalkingRunning);
			double usersHeight = 0.0;

			var timeSortDescriptor = new NSSortDescriptor (HKSample.SortIdentifierEndDate, false);
			var query = new HKSampleQuery (heightType, new NSPredicate (IntPtr.Zero), 1, new NSSortDescriptor[] { timeSortDescriptor },
				(HKSampleQuery resultQuery, HKSample[] results, NSError error) => {

					HKQuantity quantity = null;
					string resultString = string.Empty;
					if (results.Length != 0) {
						resultString = results [results.Length - 1].ToString();
						HealthKitDataContext.ActiveHealthKitData.DistanceReadings.TotalDistanceOfLastRecording = ParseStringResultToDuble(resultString);
						Console.WriteLine(string.Format("value of Fetched: {0}", ParseStringResultToDuble(resultString)));
					}

				});
			m_healthKitStore.ExecuteQuery (query);
			Console.WriteLine(string.Format("Total walked last recording: ", usersHeight));
			return usersHeight;
		}



		public async Task<double> QueryLastRegistratedSteps()
		{

			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			double usersHeight = 0.0;

			var timeSortDescriptor = new NSSortDescriptor (HKSample.SortIdentifierEndDate, false);
			var query = new HKSampleQuery (heightType, new NSPredicate (IntPtr.Zero), 1, new NSSortDescriptor[] { timeSortDescriptor },
				(HKSampleQuery resultQuery, HKSample[] results, NSError error) => {

					HKQuantity quantity = null;
					string resultString = string.Empty;
					if (results.Length != 0) {
						resultString = results [results.Length - 1].ToString();
						HealthKitDataContext.ActiveHealthKitData.DistanceReadings.TotalStepsOfLastRecording = ParseStringResultToDuble(resultString);
						Console.WriteLine(string.Format("value of Fetched: {0}", ParseStringResultToDuble(resultString)));
					}

				});
			m_healthKitStore.ExecuteQuery (query);
			Console.WriteLine(string.Format("Total steps last recording: ", usersHeight));
			return usersHeight;
		}

		private double ParseStringResultToDuble(string result)
		{
			double height = 0;
			var resultAsArray = result.Split (null);
			bool tryParse = double.TryParse(resultAsArray[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out height);

			if (tryParse)
				return height;
			else {
				return height;
			}
		}

		public async Task<string> QueryTotalFlights()
		{
			var flightsCount = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.FlightsClimbed);
			var sumOptions = HKStatisticsOptions.CumulativeSum;
			string resultString = string.Empty;
			var query = new HKStatisticsQuery(flightsCount, new NSPredicate (IntPtr.Zero), sumOptions,(HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {
				if (results != null) {
					var quantitySample = results;
					var quantity = quantitySample.SumQuantity();

				    resultString = quantity.ToString();;
					HealthKitDataContext.ActiveHealthKitData.DistanceReadings.TotalFlightsClimed = quantity.ToString();
					Console.WriteLine(string.Format("totally walked {0} flights",quantity.ToString()));

				}

			});
			await Task.Factory.StartNew(() =>HealthKitStore.ExecuteQuery (query));
			return resultString;
		}

		public async Task<string> QueryDateOfBirth()
		{
			NSError error;
			string resultString = string.Empty;
			await Task.Factory.StartNew(() =>resultString = m_healthKitStore.GetDateOfBirth (out error).ToString ());
			HealthKitDataContext.ActiveHealthKitData.DateOfBirth = resultString;
			Console.WriteLine(resultString);
			return resultString;

		}

		public async Task<string> QueryBloodType()
		{
			NSError error;
			string resultString = string.Empty;
			await Task.Factory.StartNew(() =>resultString = m_healthKitStore.GetBloodType (out error).BloodType.ToString());
			HealthKitDataContext.ActiveHealthKitData.BloodType = resultString;
			Console.WriteLine(resultString);
			return resultString;
		}

		private string ResolveBloodTypeFromEnum(enum bloodType)
		{
				//
		}


		public async Task<string> QuerySex()
		{
			NSError error;
			string resultString = string.Empty;
			await Task.Factory.StartNew(() =>resultString = m_healthKitStore.GetBiologicalSex (out error).BiologicalSex.ToString());
			HealthKitDataContext.ActiveHealthKitData.Sex = resultString;
			Console.WriteLine(resultString);
			return resultString;
		}



	}

}
