using Microsoft.ML.Data;

namespace LuvaApp.Models
{
    public class OnnxInput
    {
        [ColumnName("float_input")]
        [VectorType(69)]
        public float[] Sensores { get; set; }
    }
}
