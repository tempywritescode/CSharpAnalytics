﻿// Copyright (c) Attack Pattern LLC.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
﻿
using CSharpAnalytics.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace CSharpAnalytics.Protocols.Measurement
{
    /// <summary>
    /// Converts MeasurementActivities into key/value pairs that will form the Measurement Protocol URIs generated.
    /// </summary>
    internal class MeasurementActivityTracker
    {
        private TransactionActivity lastTransaction;

        /// <summary>
        /// Turn an IMeasurementActivity into the key/value pairs necessary for building
        /// the URI to track with Measurement Protocol.
        /// </summary>
        /// <param name="activity">Activity to turn into key/value pairs.</param>
        /// <returns>Enumerable of key/value pairs representing the activity.</returns>
        internal IEnumerable<KeyValuePair<string, string>> GetActivityParameters(IMeasurementActivity activity)
        {
            if (activity is AppViewActivity)
                return GetParameters((AppViewActivity)activity);
            if (activity is ContentViewActivity)
                return GetParameters((ContentViewActivity)activity);
            if (activity is CampaignActivity)
                return GetParameters((CampaignActivity)activity);
            if (activity is ExceptionActivity)
                return GetParameters((ExceptionActivity)activity);
            if (activity is EventActivity)
                return GetParameters((EventActivity)activity);
            if (activity is TimedEventActivity)
                return GetParameters((TimedEventActivity)activity);
            if (activity is SocialActivity)
                return GetParameters((SocialActivity)activity);
            if (activity is TransactionActivity)
            {
                var transaction = (TransactionActivity) activity;
                lastTransaction = transaction;
                return GetParameters(transaction);
            }
            if (activity is TransactionItemActivity)
                return GetParameters((TransactionItemActivity)activity);

            Debug.Assert(false, "Unknown Activity type");
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Obtain the key/value pairs for a ContentViewActivity.
        /// </summary>
        /// <param name="appView">AppViewActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this ContentViewActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(AppViewActivity appView)
        {
            yield return KeyValuePair.Create("t", "appview");

            foreach (var pair in GetSharedParameters(appView))
                yield return pair;
        }

        /// <summary>
        /// Obtain the key/value pairs for a ContentViewActivity.
        /// </summary>
        /// <param name="contentView">ContentViewActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this ContentViewActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(ContentViewActivity contentView)
        {
            yield return KeyValuePair.Create("t", "pageview");

            foreach (var pair in GetSharedParameters(contentView))
                yield return pair;
        }

        /// <summary>
        /// Obtain the key/value pairs shared by all ContentViewActivity classes.
        /// </summary>
        /// <param name="contentView">ContentViewActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this ContentViewActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetSharedParameters(ContentViewActivity contentView)
        {
            if (contentView.DocumentLocation != null)
                yield return KeyValuePair.Create("dl", contentView.DocumentLocation.OriginalString);

            if (!String.IsNullOrEmpty(contentView.DocumentHostName))
                yield return KeyValuePair.Create("dh", contentView.DocumentHostName);

            if (!String.IsNullOrEmpty(contentView.DocumentPath))
                yield return KeyValuePair.Create("dp", contentView.DocumentPath);

            if (!String.IsNullOrEmpty(contentView.DocumentTitle))
                yield return KeyValuePair.Create("dt", contentView.DocumentTitle);

            if (!String.IsNullOrEmpty(contentView.ContentDescription))
                yield return KeyValuePair.Create("cd", contentView.ContentDescription);
        }

        /// <summary>
        /// Obtain the key/value pairs for a ExceptionActivity.
        /// </summary>
        /// <param name="exception">ExceptionActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this ExceptionActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(ExceptionActivity exception)
        {
            yield return KeyValuePair.Create("t", "exception");

            yield return KeyValuePair.Create("exd", exception.Description);
            if (!exception.IsFatal)
                yield return KeyValuePair.Create("exf", "0");
        }

        /// <summary>
        /// Obtain the key/value pairs for a CampaignActivity.
        /// </summary>
        /// <param name="campaign">CampaignActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this CampaignActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(CampaignActivity campaign)
        {
            if (!String.IsNullOrEmpty(campaign.Name))
                yield return KeyValuePair.Create("cn", campaign.Name);

            if (!String.IsNullOrEmpty(campaign.Source))
                yield return KeyValuePair.Create("cs", campaign.Source);

            if (!String.IsNullOrEmpty(campaign.Medium))
                yield return KeyValuePair.Create("cm", campaign.Medium);

            if (!String.IsNullOrEmpty(campaign.Term))
                yield return KeyValuePair.Create("ck", campaign.Term);

            if (!String.IsNullOrEmpty(campaign.Content))
                yield return KeyValuePair.Create("cc", campaign.Content);
        }

        /// <summary>
        /// Obtain the key/value pairs for an EventActivity.
        /// </summary>
        /// <param name="event">EventActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this EventActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(EventActivity @event)
        {
            yield return KeyValuePair.Create("t", "event");

            if (!String.IsNullOrWhiteSpace(@event.Category))
                yield return KeyValuePair.Create("ec", @event.Category);

            if (!String.IsNullOrWhiteSpace(@event.Action))
                yield return KeyValuePair.Create("ea", @event.Action);

            if (!String.IsNullOrWhiteSpace(@event.Label))
                yield return KeyValuePair.Create("el", @event.Label);

            if (@event.Value.HasValue)
                yield return KeyValuePair.Create("ev", @event.Value.Value.ToString(CultureInfo.InvariantCulture));

            if (@event.NonInteraction)
                yield return KeyValuePair.Create("ni", "1");
        }

        /// <summary>
        /// Obtain the key/value pairs for a SocialActivity.
        /// </summary>
        /// <param name="social">SocialActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this SocialActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(SocialActivity social)
        {
            yield return KeyValuePair.Create("t", "social");

            yield return KeyValuePair.Create("sn", social.Network);
            yield return KeyValuePair.Create("sa", social.Action);
            yield return KeyValuePair.Create("st", social.Target);
        }

        /// <summary>
        /// Obtain the key/value pairs for a TimedEventActivity.
        /// </summary>
        /// <param name="timedEvent">TimedEventActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this TimedEventActivity.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetParameters(TimedEventActivity timedEvent)
        {
            yield return KeyValuePair.Create("t", "timing");

            if (!String.IsNullOrWhiteSpace(timedEvent.Category))
                yield return KeyValuePair.Create("utc", timedEvent.Category);

            if (!String.IsNullOrWhiteSpace(timedEvent.Variable))
                yield return KeyValuePair.Create("utv", timedEvent.Variable);

            if (!String.IsNullOrWhiteSpace(timedEvent.Label))
                yield return KeyValuePair.Create("utl", timedEvent.Label);

            yield return KeyValuePair.Create("utt", timedEvent.Time.TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Obtain the key/value pairs for a TransactionActivity.
        /// </summary>
        /// <param name="transaction">TransactionActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this TransactionActivity.</returns>
        internal IEnumerable<KeyValuePair<string, string>> GetParameters(TransactionActivity transaction)
        {
            yield return KeyValuePair.Create("t", "transaction");
            yield return KeyValuePair.Create("ti", transaction.OrderId);

            if (!String.IsNullOrWhiteSpace(transaction.StoreName))
                yield return KeyValuePair.Create("ta", transaction.StoreName);

            if (transaction.OrderTotal != Decimal.Zero)
                yield return KeyValuePair.Create("tr", transaction.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));

            if (transaction.ShippingCost != Decimal.Zero)
                yield return KeyValuePair.Create("ts", transaction.ShippingCost.ToString("0.00", CultureInfo.InvariantCulture));

            if (transaction.TaxCost != Decimal.Zero)
                yield return KeyValuePair.Create("tt", transaction.TaxCost.ToString("0.00", CultureInfo.InvariantCulture));

            if (!String.IsNullOrWhiteSpace(transaction.Currency))
                yield return KeyValuePair.Create("cu", transaction.Currency);
        }

        /// <summary>
        /// Obtain the key/value pairs for a TransactionItemActivity.
        /// </summary>
        /// <param name="item">TransactionItemActivity to turn into key/value pairs.</param>
        /// <returns>Key/value pairs representing this TransactionItemActivity.</returns>
        internal IEnumerable<KeyValuePair<string, string>> GetParameters(TransactionItemActivity item)
        {
            yield return KeyValuePair.Create("t", "item");

            yield return KeyValuePair.Create("ti", lastTransaction.OrderId);

            if (item.Price != Decimal.Zero)
                yield return KeyValuePair.Create("ip", item.Price.ToString("0.00", CultureInfo.InvariantCulture));

            if (item.Quantity != 0)
                yield return KeyValuePair.Create("iq", item.Quantity.ToString(CultureInfo.InvariantCulture));

            if (!String.IsNullOrWhiteSpace(item.Code))
                yield return KeyValuePair.Create("ic", item.Code);

            if (!String.IsNullOrWhiteSpace(item.Name))
                yield return KeyValuePair.Create("in", item.Name);

            if (!String.IsNullOrEmpty(item.Variation))
                yield return KeyValuePair.Create("iv", item.Variation);

            if (!String.IsNullOrWhiteSpace(lastTransaction.Currency))
                yield return KeyValuePair.Create("cu", lastTransaction.Currency);
        }
    }
}