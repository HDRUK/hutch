import {
  Button,
  AlertDialog,
  AlertDialogOverlay,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogBody,
} from "@chakra-ui/react";
import { FaTrash } from "react-icons/fa";

export const DeleteModal = ({ title, body, isOpen, onClose, onDelete }) => (
  <AlertDialog isOpen={isOpen} onClose={onClose}>
    <AlertDialogOverlay />
    <AlertDialogContent>
      <AlertDialogHeader fontSize="lg" fontWeight="bold">
        {title}
      </AlertDialogHeader>

      <AlertDialogBody>{body}</AlertDialogBody>
      <AlertDialogFooter>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          leftIcon={<FaTrash />}
          colorScheme="red"
          onClick={onDelete}
          ml={3}
        >
          Delete
        </Button>
      </AlertDialogFooter>
    </AlertDialogContent>
  </AlertDialog>
);
