from flask import Flask, request, Response, send_file
from AIHelper.predictModel import predictByModel
from AIHelper.trainModel import trainModel

app = Flask(__name__)

@app.route("/predict", methods=['POST'])
def predict():
    modelToUse = request.get_json()['model']
    valuesToUse = request.get_json()['values']
    return str(predictByModel(modelToUse, valuesToUse)[0])

@app.route('/trainModel', methods=['POST'])
def train():    
    modelName = request.get_json()['model']
    accuracy = trainModel(modelName)
    return str(accuracy)

@app.route('/clearData')
def clear_data():    
    clear_file()
    return 'OK', 200

@app.route('/download')
def download_csv():
    csv_file_path = 'dataForTrain.csv'

    with open(csv_file_path, 'r') as file:
        csv_data = file.read()

    response = Response(csv_data, content_type='text/csv')
    response.headers['Content-Disposition'] = 'attachment; filename=data_for_training.csv'
    return response

@app.route('/downloadOnnx')
def download_onnx():
    fileName = request.get_json()['fileName']
    onnx_model_path = 'TrainedModels/' + fileName
    return send_file(onnx_model_path, as_attachment=True)

@app.route("/receiveValues", methods=['POST'])
def save_values_to_file():
    write_on_file(request.get_json()['values'])
    return '', 204

def write_on_file(values):
    f = open("dataForTrain.csv", "a")
    charsToRemove = "[]' "
    cleanedValues = ''.join([char for char in str(values) if char not in charsToRemove])
    f.write(cleanedValues)
    f.write("\n")
    f.close()

def clear_file():
    with open("dataForTrain.csv", 'w') as file:
        file.write('')