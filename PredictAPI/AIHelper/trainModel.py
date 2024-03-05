import pandas as pd
import joblib
import os

from sklearn.svm import SVC
from sklearn.metrics import accuracy_score

from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

def trainModel(modelName):    
    X_train, X_test, y_train, y_test = setTrainTest()
    model = fitModel(X_train, y_train)

    createJoblib(model, modelName)
    createOnnx(X_train, model, modelName)
    
    y_pred_svc = model.predict(X_test)
    accuracy = accuracy_score(y_test, y_pred_svc)
    return accuracy

def fitModel(X_train, y_train):
    model = SVC()
    model.fit(X_train, y_train)
    return model

def createJoblib(model, modelName):
    current_directory = os.getcwd() + '\TrainedModels\\' + modelName + '.joblib'
    joblib.dump(model, current_directory)

def createOnnx(X_train, model, modelName):
    initial_type = [('float_input', FloatTensorType([None, X_train.shape[1]]))]
    onx = convert_sklearn(model, initial_types=initial_type)
    current_directory = os.getcwd() + '\TrainedModels\\' + modelName + '.onnx'
    with open(current_directory, "wb") as f:
        f.write(onx.SerializeToString())
    
def setTrainTest():    
    current_directory = os.getcwd()
    df = pd.read_csv(current_directory + '\dataForTrain.csv')

    train = df.iloc[:-21]
    test = df.iloc[-21:]
    train = train.sample(frac=1.0, random_state=42)
    test = test.sample(frac=1.0, random_state=42)

    X_train = train.iloc[:, :-1]
    X_test = test.iloc[:, :-1]
    y_train = train.iloc[:,-1:]
    y_test = test.iloc[:,-1:]

    return X_train, X_test, y_train, y_test