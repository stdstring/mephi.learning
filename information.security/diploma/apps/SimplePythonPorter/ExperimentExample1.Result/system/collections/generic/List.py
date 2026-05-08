from typing import TypeVar, Generic
import system
from system.collections.generic.IList import IList

T = TypeVar('T')

class List(Generic[T], IList[T]):
    def __init__(self):
        self._items: list[T] = []
    
    @property
    def Count(self) -> int:
        return len(self._items)
    
    def __getitem__(self, index: int) -> T:
        if index < 0 or index >= len(self._items):
            raise ArgumentOutOfRangeException("Index out of range")
        return self._items[index]
    
    def __setitem__(self, index: int, value: T) -> None:
        if index < 0 or index >= len(self._items):
            raise ArgumentOutOfRangeException("Index out of range")
        self._items[index] = value
    
    def Add(self, item: T) -> None:
        self._items.append(item)
    
    def Clear(self) -> None:
        self._items.clear()
    
    def Contains(self, item: T) -> bool:
        return item in self._items
    
    def IndexOf(self, item: T) -> int:
        try:
            return self._items.index(item)
        except ValueError:
            return -1
    
    def Insert(self, index: int, item: T) -> None:
        if index < 0 or index > len(self._items):
            raise ArgumentOutOfRangeException("Index out of range")
        self._items.insert(index, item)
    
    def Remove(self, item: T) -> bool:
        try:
            self._items.remove(item)
            return True
        except ValueError:
            return False
    
    def RemoveAt(self, index: int) -> None:
        if index < 0 or index >= len(self._items):
            raise ArgumentOutOfRangeException("Index out of range")
        del self._items[index]
    
    def ToArray(self) -> list[T]:
        return self._items.copy()
    
    def __iter__(self):
        return iter(self._items)
