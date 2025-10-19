import cv2 
from Lib.Core import Engine 
from Lib.ImageProcessingSteps import MedianBlurProcessingStep

my_photo = cv2.Imread('~/Tools/Level_4/PacketImageAnalysisWithOpenCV/Photos/MyPhoto1.jpg')
core = Engine()
core.steps.append(MedianBlurProcessingStep(5))
res, history = core.process(my_photo)

i = 1
for info in history:
    cv2.imshow('image' + str(i), info.image)
    i = i + 1
cv2.imshow('res', res.image)

#cv2.imshow('origin', info.image) # вывод исходника
#cv2.imshow('res', info.filtered_image) # вывод итога 

cv2.waitKey()
cv2.destroyllWindows()


