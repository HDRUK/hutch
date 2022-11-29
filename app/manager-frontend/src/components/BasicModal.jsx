import {
  Button,
  AlertDialog,
  AlertDialogOverlay,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogBody,
} from "@chakra-ui/react";
import { FaTimes, FaRegCheckCircle } from "react-icons/fa";

export const BasicModal = ({
  title, // Modal title
  body,
  isOpen,
  onClose,
  onAction, // onClick event action for Ok/primary button
  isLoading,
  actionBtnCaption = "Ok", // caption for the primary button
  actionBtnColorScheme, // color scheme for the primary button
  cancelBtnEnable = true, // By default, Cancel/Secondary button is visible/enabled
  cancelBtnCaption = "Cancel", // caption for the Cancel/Secondary button
  cancelBtnAction = onClose,
  closeOnOverlayClick = true,
}) => (
  <AlertDialog
    closeOnOverlayClick={closeOnOverlayClick}
    isOpen={isOpen}
    onClose={cancelBtnAction}
  >
    <AlertDialogOverlay />
    <AlertDialogContent>
      <AlertDialogHeader fontSize="lg" fontWeight="bold">
        {title}
      </AlertDialogHeader>
      <AlertDialogBody>{body}</AlertDialogBody>
      <AlertDialogFooter>
        {cancelBtnEnable && (
          <Button onClick={cancelBtnAction} leftIcon={<FaTimes />}>
            {cancelBtnCaption}
          </Button>
        )}
        <Button
          leftIcon={<FaRegCheckCircle />}
          colorScheme={actionBtnColorScheme}
          onClick={onAction}
          ml={3}
          isLoading={isLoading}
        >
          {actionBtnCaption}
        </Button>
      </AlertDialogFooter>
    </AlertDialogContent>
  </AlertDialog>
);
