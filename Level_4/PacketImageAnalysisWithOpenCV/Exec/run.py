import cv2 
from Lib.Core import Engine 
from Lib.ImageProcessingSteps import MedianBlurProcessingStep

my_photo = cv2.Imread('~/Tools/Level_4/PacketImageAnalysisWithOpenCV/Photos/MyPhoto1.jpg')
core = Engine()
core.steps.append(MedianBlurProcessingStep(5))
info = core.processing(my_photo)

cv2.imshow('origin', info.image) # вывод исходника
cv2.imshow('res', info.filtered_image) # вывод итога 

cv2.waitKey()
cv2.destroyllWindows()


