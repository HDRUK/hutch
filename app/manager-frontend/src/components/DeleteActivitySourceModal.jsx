import {
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
} from "@chakra-ui/react";

export const DeleteActivitySourceModal = ({
  isOpen,
  onClose,
  id,
  onDeleteSource,
}) => (
  <Modal isOpen={isOpen} onClose={onClose}>
    <ModalOverlay />
    <ModalContent>
      <ModalHeader>Delete Activity Source {id}</ModalHeader>
      <ModalCloseButton />
      <ModalBody>
        Are you sure you want to delete this activity source? You will not be
        able to reverse this
      </ModalBody>

      <ModalFooter>
        <Button variant="ghost" mr={3} onClick={onClose}>
          Close
        </Button>
        <Button colorScheme="red" onClick={onDeleteSource}>
          Delete
        </Button>
      </ModalFooter>
    </ModalContent>
  </Modal>
);
