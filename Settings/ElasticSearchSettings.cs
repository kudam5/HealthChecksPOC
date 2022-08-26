namespace HealthMonitorPOC.Web.Settings
{
    public class ElasticSearchSettings
    {
        public string Endpoint { get; set; }
        public string IndexName { get; set; }
        public ElasticAggregateQuery ElasticAggregateQuery { get; set; }
        public string ElasticQueryPath { get; set; }      
    }

    public class Aggs
    {
        public AvgGrade avg_grade { get; set; }
    }

    public class Avg
    {
        public string field { get; set; }
    }

    public class AvgGrade
    {
        public Avg avg { get; set; }
    } 

    public class ElasticAggregateQuery
    {
        public Aggs aggs { get; set; }
    }

    public class Aggregations
    {
        public AvgGradeValue avg_grade { get; set; }
    }

    public class AvgGradeValue
    {
        public double value { get; set; }
    }

    public class Hits
    {
        public Total total { get; set; }
        public object max_score { get; set; }
        public List<object> hits { get; set; }
    }

    public class ElasticQueryResult
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hits hits { get; set; }
        public Aggregations aggregations { get; set; }
    }

    public class Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

}
