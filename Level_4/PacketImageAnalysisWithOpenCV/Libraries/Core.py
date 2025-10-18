class Engine:
    """
    Движок
    """

    def __init__(self):
        """Конструктор"""

        self.steps = []

    def process(self, image):
        current_info = ImageInfo(image)
        for step in self.steps:
            current_info = step.process(current_info)

        return current_info

        pass
''' 
Перенесен в отдельный файл


class ImageProcessingSteps:
    """
    Шаг обработки изображений
    """

    def process(self, info):
        """Выполнить обработку"""
    pass
    
'''
class ImageInfo:
    """
    Содержимое картинки, включая результаты обработки 
    """
    def __init__(self, image):
        """Конструктор"""

        self.image = image
    pass


class MedianBlurProcessingStep(ImageProcessingSteps):

    def __init__(self, ksize):
        # ksize - размер ядра фильтра 
        self.ksize = ksize
    
    def process(self, info):

        median_image = cv2.medianBlur(info.image, self.ksize)
        info.filtered_image = median_image

        return info

