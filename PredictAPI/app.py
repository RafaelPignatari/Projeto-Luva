from flask import Flask, request, Response, send_file
from AIHelper.trainModel import predictByBestModel, trainAllModel, predictByAllModels

app = Flask(__name__)

@app.route("/predict", methods=['POST'])
def predict():
    isBestModel = request.get_json()['isBestModel']
    valuesToUse = request.get_json()['values']
    if(isBestModel):
        return str(predictByBestModel(valuesToUse))

    return str(predictByAllModels(valuesToUse))

@app.route('/trainModel', methods=['POST'])
def train():    
    accuracy = trainAllModel()
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
    charsToRemove = "[]' "
    cleanedValues = ''.join([char for char in str(values) if char not in charsToRemove])
    if(cleanedValues[-1] == ','):
        return
    
    f = open("dataForTrain.csv", "a")
    f.write(cleanedValues)
    f.write("\n")
    f.close()

def clear_file():
    with open("dataForTrain.csv", 'w') as file:
        file.write('')