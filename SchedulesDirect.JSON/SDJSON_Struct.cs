﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SchedulesDirect
{
    // ========================
    // Token command structures
    // ========================
    [DataContract]
    public class SDTokenMessage
    {
        [DataMember]
        public string username;
        [DataMember]
        public string password;
    }

    [DataContract]
    public class SDTokenResponse
    {
        [DataMember]
        public int code;
        [DataMember]
        public string message;
        [DataMember]
        public string datetime;
        [DataMember]
        public string token;
        [DataMember]
        public string response;
    }

    // =========================
    // Status command structures
    // =========================
    [DataContract]
    public class SDStatusResponse
    {
        [DataMember]
        public SDAccount account;
        [DataMember]
        public SDLineUps[] lineups;
        [DataMember]
        public string lastDataUpdate;
        [DataMember]
        public string[] notifications;
        [DataMember]
        public SDSystemStatus[] systemStatus;
        [DataMember]
        public string serverID;
        [DataMember]
        public string datetime;
        [DataMember]
        public int code;

        [DataContract]
        public class SDAccount
        {
            [DataMember]
            public string expires;
            [DataMember]
            public string[] messages;
            [DataMember]
            public int maxLineups;
        }

        [DataContract]
        public class SDLineUps
        {
            [DataMember]
            public string lineup;
            [DataMember]
            public string modified;
            [DataMember]
            public string url;
            [DataMember]
            public bool isDeleted;
        }

        [DataContract]
        public class SDSystemStatus
        {
            [DataMember]
            public string date;
            [DataMember]
            public string status;
            [DataMember]
            public string message;
        }
    }

    // ==========================
    // Version command structures
    // ==========================
    [DataContract]
    public class SDVersionResponse
    {
        [DataMember]
        public string response;
        [DataMember]
        public int code;
        [DataMember]
        public string client;
        [DataMember]
        public string version;
        [DataMember]
        public string serverID;
        [DataMember]
        public string datetime;
        [DataMember]
        public string message;
    }

    // ============================
    // Available command structures
    // ============================
    [DataContract]
    public class SDAvailableResponse
    {
        [DataMember]
        public string type;
        [DataMember]
        public string description;
        [DataMember]
        public string uri;
    }

    // ================================
    // Countries sub-command structures
    // ================================
    public class SDCountries
    {
        public List<Continent> continents;

        public SDCountries()
        {
            continents = new List<Continent>();
        }

        public class Continent
        {
            public string continentname;
            public List<Country> countries;

            public Continent()
            {
                countries = new List<Country>();
            }
        }

        public class Country
        {
            public string fullName;
            public string shortName;
            public string postalCodeExample;
            public string postalCode;
            public bool onePostalCode;
        }
    }
}