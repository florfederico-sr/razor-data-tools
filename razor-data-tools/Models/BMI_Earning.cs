using System;
using System.Linq;
using SoftCircuits.CsvParser;

namespace razor_data_tools.Models
{
    public class BMI_Earning
    {
        
        public string PERIOD { get; set; } = string.Empty;
        public string W_OR_P { get; set; } = string.Empty;
        public string PARTICIPANT_NAME { get; set; } = string.Empty;
        public string PARTICIPANT_NUM { get; set; } = string.Empty;
        public string IP_NUM { get; set; } = string.Empty;
        public string TITLE_NAME { get; set; } = string.Empty;
        public string TITLE_NUM { get; set; } = string.Empty;
        public string PERF_SOURCE { get; set; } = string.Empty;
        public string COUNTRY_OF_PERFORMANCE { get; set; } = string.Empty;
        public string SHOW_NAME { get; set; } = string.Empty;
        public string EPISODE_NAME { get; set; } = string.Empty;
        public string SHOW_NUM { get; set; } = string.Empty;
        public string USE_CODE { get; set; } = string.Empty;
        public string TIMING { get; set; } = string.Empty;
        public string PARTICIPANT_PCT { get; set; } = string.Empty;
        public string PERF_COUNT { get; set; } = string.Empty;
        public string BONUS_LEVEL { get; set; } = string.Empty;
        public string ROYALTY_AMOUNT { get; set; } = string.Empty;
        public string WITHHOLD { get; set; } = string.Empty;
        public string PERF_PERIOD { get; set; } = string.Empty;
        public string CURRENT_ACTIVITY_AMT { get; set; } = string.Empty;
        public string HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS { get; set; } = string.Empty;
        public string STANDARDS_OR_TV_NET_THEME_BONUS { get; set; } = string.Empty;
        public string FOREIGN_SOCIETY_ADJUSTMENT { get; set; } = string.Empty;
        public string COMPANY_CODE { get; set; } = string.Empty;
        public string COMPANY_NAME { get; set; } = string.Empty;

    }

    /// <summary>
    /// Simple class for select list
    /// </summary>
    public class BMI_Earnings_Customer
    {
        public string PARTICIPANT_NAME { get; set; }
        public string PARTICIPANT_NUM { get; set; }
    }

}
