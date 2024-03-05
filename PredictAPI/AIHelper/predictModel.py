import joblib
import numpy
import os



def predictByModel(model, data_to_predict):
    loaded_model = ''
    current_directory = os.getcwd() + '\TrainedModels'
    model = model.lower()
    if model == 'svc':        
        loaded_model = joblib.load(current_directory + '\svm.joblib')
    elif model == 'random forest':
        loaded_model = joblib.load( current_directory + '\RandomForest.joblib')
    elif model == 'decision tree':
        loaded_model = joblib.load(current_directory + '\DecisionTree.joblib')
    elif model == 'logistic regression': 
        loaded_model = joblib.load(current_directory + '\logisticRegression.joblib')
    else:
        return 'Invalid model'
    
    predictions = loaded_model.predict([numpy.asarray(data_to_predict)])
    return predictions