import {
  Heading,
  Text,
  LinkBox,
  LinkOverlay,
  Box,
  Divider,
  Center,
  Stack,
  useColorModeValue,
  IconButton,
  HStack,
  VStack,
  useDisclosure,
  useToast,
  Button,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import { FaTrash, FaLink } from "react-icons/fa";
import { Link } from "react-router-dom";
import { useBackendApi } from "contexts/BackendApi";
import { useEffect, useState } from "react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { BasicModal } from "components/BasicModal";

export const UserSummary = ({
  userId,
  name, //user display name
  username, // username
  isUserActive, // user status
  href, // user page
  onDelete,
  ...p
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const [activationOrPwdResetLink, setActivationOrPwdResetLink] = useState();
  const [selectedAction, setSelectedAction] = useState();
  const { account } = useBackendApi();

  const getNameInitials = () => {
    // get name initials
    const splitName = name.trim().split(" "); // split name into an array
    let initials = ""; // empty initials
    splitName.forEach((name) => {
      initials = `${initials}${name.charAt(0).toUpperCase()}`; // append first char to 'initials'
    });
    return initials;
  };

  const {
    isOpen: isDisplayLinkOpen,
    onOpen: onDisplayLinkOpen,
    onClose: onDisplayLinkClose,
  } = useDisclosure(); // Handle the modal that displays generated link

  const toast = useToast();
  const displayToast = ({
    // toast configured for the User summary
    position = "top",
    title,
    status,
    duration = "900",
    isClosable = true,
  }) =>
    toast({
      position: position,
      title: title,
      status: status,
      duration: duration,
      isClosable: isClosable,
    });

  const actionMenu = {
    accountActivate: {
      // applicable for handling account activation link
      name: "activationLink", // match with object key of the response. For e.g. activationLink: "Activation link here"
      title: "Account Activation",
      apiAction: async () =>
        account.generateActivationLink({ id: userId }).json(), // function that triggers api call
    },
    passwordReset: {
      // applicable for handling password reset link
      name: "passwordResetLink",
      title: "Password reset",
      apiAction: async () =>
        account.generatePasswordResetLink({ id: userId }).json(),
    },
  };

  useEffect(() => {
    const generateLink = async () => {
      try {
        setIsLoading(true);
        const actionResponse = await selectedAction.apiAction(); // get requested link
        setActivationOrPwdResetLink(actionResponse[selectedAction.name]); // update the state with the link received
        displayToast({
          title: `New ${selectedAction.title} link generated`,
          status: "success",
          duration: 900,
        });
        setIsLoading(false);
      } catch (e) {
        console.error(e);
        setFeedback("Something went wrong!");
      }
    };
    selectedAction && generateLink();
    onDisplayLinkOpen();
  }, [selectedAction]);

  const ModalDisplayLink = // Display activation/password reset link with only an OK button
    (feedback || selectedAction) && (
      <BasicModal
        body={
          feedback ? (
            <Alert status="error">
              <AlertIcon />
              {feedback}
            </Alert>
          ) : (
            <Formik
              enableReinitialize
              initialValues={{
                [selectedAction.name]: activationOrPwdResetLink, // get Activation/Password reset link from the state
              }}
            >
              <Form noValidate>
                <VStack align="stretch" spacing={4}>
                  <FormikInput
                    label={`${selectedAction.title} Link`}
                    name={selectedAction.name}
                    type="readOnly"
                  />
                  <Alert status="info">
                    <AlertIcon />
                    Please copy the {selectedAction.title} link and pass it to
                    the user to complete the {selectedAction.title} process.
                  </Alert>
                </VStack>
              </Form>
            </Formik>
          )
        }
        title={selectedAction.title}
        actionBtnCaption="Ok"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={onDisplayLinkClose}
        isOpen={isDisplayLinkOpen}
        onClose={onDisplayLinkClose}
        closeOnOverlayClick={feedback ? true : false} // disable closing the modal when clicked on overlay
        cancelBtnEnable={feedback ? true : false} // display cancel if error else hide cancel button
      />
    );

  return (
    <Center py={2}>
      <LinkBox width="700px">
        <Box
          width="700px"
          bg={useColorModeValue("white", "gray.900")}
          boxShadow="lg"
          rounded="md"
          p={5}
          overflow="hidden"
          {...p}
          _hover={{
            transition: "0.3s",
            transform: "translateY(-2px)",
            boxShadow: "md",
          }}
        >
          <Stack>
            <Text
              color={"blue.500"}
              textTransform={"uppercase"}
              fontWeight={800}
              fontSize={"sm"}
              letterSpacing={1.1}
            >
              User
            </Text>
            <Divider />
            {onDelete && (
              <IconButton
                h={5}
                w={5}
                icon={<FaTrash />}
                alignSelf={"flex-end"}
                style={{ background: "transparent" }}
                color={"red.500"}
                onClick={onDelete}
              />
            )}

            <HStack alignItems={"center"} spacing={3}>
              <Heading
                display="flex"
                justifyContent="center"
                alignItems="center"
                bg="gray.200"
                color="blue.900"
                borderColor="gray.300"
                borderWidth={1}
                borderRadius="50%"
                h="50px"
                w="50px"
                fontSize={"2xl"}
              >
                {isUserActive ? getNameInitials() : "?"}
              </Heading>
              <VStack alignItems="flex-start" spacing={0.2}>
                <Heading
                  color={useColorModeValue("gray.700", "white")}
                  fontSize={"2xl"}
                  fontFamily={"body"}
                >
                  {isUserActive ? name : username}
                </Heading>
                <Text color="gray.600" fontSize="sm" align="left" width="full">
                  {isUserActive ? username : "⚠️"}
                </Text>
              </VStack>
            </HStack>
            {!isUserActive && (
              <HStack pt="10px" justifyContent="end">
                <Button
                  size="sm"
                  colorScheme="green"
                  leftIcon={<FaLink />}
                  onClick={() => setSelectedAction(actionMenu.accountActivate)}
                >
                  Generate {actionMenu.accountActivate.title}
                </Button>
              </HStack>
            )}
            {isUserActive && (
              <HStack pt="10px" justifyContent="end">
                <Button
                  size="sm"
                  colorScheme="blue"
                  leftIcon={<FaLink />}
                  onClick={() => setSelectedAction(actionMenu.passwordReset)}
                >
                  Generate {actionMenu.passwordReset.title}
                </Button>
              </HStack>
            )}
          </Stack>
        </Box>
      </LinkBox>
      {ModalDisplayLink}
    </Center>
  );
};
