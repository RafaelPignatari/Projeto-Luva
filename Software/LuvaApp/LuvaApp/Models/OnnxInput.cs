using Microsoft.ML.Data;

namespace LuvaApp.Models
{
    public class OnnxInput
    {
        [ColumnName("float_input")]
        [VectorType(40)]
        public float[] Sensores { get; set; }
    }
}
