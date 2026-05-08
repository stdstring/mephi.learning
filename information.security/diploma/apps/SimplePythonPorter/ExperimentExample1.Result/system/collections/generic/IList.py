from typing import TypeVar, Generic
from abc import ABC, abstractmethod

T = TypeVar('T')

class IList(Generic[T], ABC):
    @property
    @abstractmethod
    def Count(self) -> int:
        pass
    
    @abstractmethod
    def __getitem__(self, index: int) -> T:
        pass
    
    @abstractmethod
    def __setitem__(self, index: int, value: T) -> None:
        pass
    
    @abstractmethod
    def Add(self, item: T) -> None:
        pass
    
    @abstractmethod
    def Clear(self) -> None:
        pass
    
    @abstractmethod
    def Contains(self, item: T) -> bool:
        pass
    
    @abstractmethod
    def IndexOf(self, item: T) -> int:
        pass
    
    @abstractmethod
    def Insert(self, index: int, item: T) -> None:
        pass
    
    @abstractmethod
    def Remove(self, item: T) -> bool:
        pass
    
    @abstractmethod
    def RemoveAt(self, index: int) -> None:
        pass
    
    @abstractmethod
    def ToArray(self) -> list[T]:
        pass
    
    @abstractmethod
    def __iter__(self):
        pass