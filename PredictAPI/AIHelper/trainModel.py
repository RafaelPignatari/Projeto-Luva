import pandas as pd
import joblib
import os

import numpy

from sklearn.svm import SVC
from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler

from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType


scaler = StandardScaler()

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
    df = df.sort_values(by=df.columns[-1])

    train, test = train_test_split(df, test_size=0.2, stratify=df[df.columns[-1]])

    X_train = train.iloc[:, :-1]
    X_train = pd.DataFrame(scaler.fit_transform(X_train), columns=X_train.columns)
    X_test = test.iloc[:, :-1]
    X_test = pd.DataFrame(scaler.transform(X_test), columns=X_test.columns)
    y_train = train.iloc[:,-1:]
    y_test = test.iloc[:,-1:]

    return X_train, X_test, y_train, y_test

def predictByModel(model, data_to_predict):
    loaded_model = ''
    current_directory = os.getcwd() + '\TrainedModels'
    model = model.lower()
    if model == 'svm':        
        loaded_model = joblib.load(current_directory + '\svm.joblib')
    elif model == 'random forest':
        loaded_model = joblib.load( current_directory + '\RandomForest.joblib')
    elif model == 'decision tree':
        loaded_model = joblib.load(current_directory + '\DecisionTree.joblib')
    elif model == 'logistic regression': 
        loaded_model = joblib.load(current_directory + '\logisticRegression.joblib')
    else:
        return 'Invalid model'
    
    print(scaler.transform(numpy.array([data_to_predict])))
    predictions = loaded_model.predict(scaler.transform(numpy.array([data_to_predict])))
    return predictions