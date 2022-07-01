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

export const DeleteModal = ({ title, body, isOpen, onClose, onDelete }) => (
  <Modal isOpen={isOpen} onClose={onClose}>
    <ModalOverlay />
    <ModalContent>
      <ModalHeader>{title}</ModalHeader>
      <ModalCloseButton />
      <ModalBody>{body}</ModalBody>
      <ModalFooter>
        <Button variant="ghost" mr={3} onClick={onClose}>
          Close
        </Button>
        <Button colorScheme="red" onClick={onDelete}>
          Delete
        </Button>
      </ModalFooter>
    </ModalContent>
  </Modal>
);
