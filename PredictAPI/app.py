from flask import Flask, jsonify, request
from TrainedModels.predictModel import predictByModel

app = Flask(__name__)

@app.route("/")
def predict():
    modelToUse = request.get_json()['model']
    valuesToUse = request.get_json()['values']
    return str(predictByModel(modelToUse, valuesToUse)[0])

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