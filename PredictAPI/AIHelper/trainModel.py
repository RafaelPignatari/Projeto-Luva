import pandas as pd
import joblib
import os
import numpy

from sklearn.svm import SVC
from sklearn.linear_model import LogisticRegression
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier

from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler

from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType


scaler = StandardScaler()
bestModel = ''

def trainModel(modelName):    
    X_train, X_test, y_train, y_test = setTrainTest()
    model = fitModel(modelName, X_train, y_train)

    createJoblib(model, modelName)
    createOnnx(X_train, model, modelName)
    
    y_pred = model.predict(X_test)
    accuracy = accuracy_score(y_test, y_pred)
    return accuracy

def fitModel(modelName, X_train, y_train):
    model = '';
    modelName = modelName.lower()

    if modelName == 'svm':        
        model = SVC(random_state=101)
    elif modelName == 'logistic regression':
        model = LogisticRegression(max_iter=3000, random_state=101)
    elif modelName == 'decision tree':
        model = DecisionTreeClassifier(random_state=101)
    elif modelName == 'random forest': 
        model = RandomForestClassifier(random_state=101)
    else:
        return 'Invalid model'
    
    model.fit(X_train, y_train)
    return model

def trainAllModel():    
    returnMessage = ''
    higherAccurracy = 0;
    for modelName in ['svm', 'random forest', 'decision tree', 'logistic regression']:
        X_train, X_test, y_train, y_test = setTrainTest()
        model = fitModel(modelName, X_train, y_train)

        createJoblib(model, modelName)
        createOnnx(X_train, model, modelName)
        
        y_pred = model.predict(X_test)
        accuracy = accuracy_score(y_test, y_pred)
        if(higherAccurracy < accuracy):
            higherAccurracy = accuracy
            global bestModel
            bestModel = modelName
        returnMessage += modelName + ' ' + str(accuracy) + ';' 

    return returnMessage

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
    df = pd.read_csv(current_directory + '/Dados/dataForTrain.csv')
    df = df.sort_values(by=df.columns[-1])

    train, test = train_test_split(df, test_size=0.2, random_state=101, stratify=df[df.columns[-1]])

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
    elif model == 'logistic regression':
        loaded_model = joblib.load( current_directory + '\logistic regression.joblib')
    elif model == 'decision tree':
        loaded_model = joblib.load(current_directory + '\decision tree.joblib')
    elif model == 'random forest': 
        loaded_model = joblib.load(current_directory + '\\random forest.joblib')
    else:
        return 'Invalid model'
    
    predictions = loaded_model.predict(scaler.transform(numpy.array([data_to_predict])))
    return predictions

def predictByAllModels(data_to_predict):    
    returnMessage = ''
    for modelName in ['svm', 'logistic regression', 'decision tree', 'random forest']:
        prediction = predictByModel(modelName, data_to_predict)[0]
        returnMessage += str(prediction) + ';' 
    return returnMessage

def predictByBestModel(data_to_predict):   
    prediction = predictByModel(bestModel, data_to_predict)[0]
    returnMessage = str(prediction) + ';' 
    return returnMessage