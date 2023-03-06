class BaseDto:

    def to_dict(self) -> dict:
        raise NotImplementedError

    @classmethod
    def from_dict(cls, dict_: dict):
        raise NotImplementedError
