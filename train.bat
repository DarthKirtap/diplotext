cd sample-env\Scripts
call activate.bat
cd ../../RL_train
mlagents-learn NICO.yaml --run-id=HEAD2
pause