import cv2 

from Lib.Core import Engine 
from Lib.ImageProcessingStepd import MedianBlurProcessingStep, TresholdProcessingStep

my_photo = cv2.imread('../Photos/6108249.jpg')
core = Engine
core.steps.append(MedianBlurProcessingStep)
core.steps.append(TresholdProcessingStep)
res, history = core.process(my_photo)

i = 1
for info in history:
    cv2.imshow('image' + str(i), info.image)
    i = i + 1
cv2.imshow('res', res.image)

cv2.waitKey()
cv2.destroyAllWindows()
