using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Transforms.Onnx;

namespace LuvaApp.Models
{
    public class OnnxOutput
    {
        [ColumnName("output_label")]
        public string[] Resultado { get; set; }

        [ColumnName("output_probability")]
        [OnnxSequenceType(typeof(IDictionary<string, float>))]
        public IEnumerable<IDictionary<string, float>> Probabilities { get; set; }
    }
}
