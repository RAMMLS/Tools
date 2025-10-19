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
            history.append(current_info)
            current_info = step.process(current_info)


        return current_info, history


class ImageInfo:
    """
    Содержимое картинки, включая результаты обработки 
    """
    def __init__(self, image):
        """Конструктор"""

        self.image = image

'''
class MedianBlurProcessingStep(ImageProcessingStep):
    """Шаг, отвечающий за предобработку типа Медианная фильтрация"""

    def __init__(self,ksize):
        """Конструктор
        ksize - размер ядра фильтра"""

        self.ksize=ksize

    def process(self,info):
        """Выполнить обработку"""

        median_image = cv2.medianBlur(info.image, self.ksize)
        info.filtered_image=median_image
        return info

'''
