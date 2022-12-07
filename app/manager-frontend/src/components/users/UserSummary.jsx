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
import { useState } from "react";
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
  const [activationLink, setActivationLink] = useState();

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
    isOpen: isGenerateActivationLinkOpen,
    onOpen: onGenerateActivationLinkOpen,
    onClose: onGenerateActivationLinkClose,
  } = useDisclosure(); // Handle Generate activation link modal

  const {
    isOpen: isDisplayActivationLinkOpen,
    onOpen: onDisplayActivationLinkOpen,
    onClose: onDisplayActivationLinkClose,
  } = useDisclosure(); // Handle Display ActivationLink modal

  const toast = useToast();
  // toast configured for the User summary
  const displayToast = ({
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

  const { users } = useBackendApi();

  const onGenerateActivationLink = async () => {
    try {
      // handle submission for generating activation link
      setIsLoading(true);
      const actionResponse = await users
        .generateActivationLink({ id: userId })
        .json(); // generate and get activation link
      if (actionResponse) {
        setActivationLink(actionResponse.activationLink); // update the state
      }
      setIsLoading(false);
      onGenerateActivationLinkClose();
      displayToast({
        title: "New activation link generted",
        status: "success",
        duration: 900,
      });
      onDisplayActivationLinkOpen();
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
    }
  };

  const ModalGenerateActivationLink = // Modal for displaying Generate Activation link
    (
      <BasicModal
        title="Generate an Account Activation Link?"
        body={
          <VStack>
            <VStack>
              {feedback && (
                <Alert status="error">
                  <AlertIcon />
                  {feedback}
                </Alert>
              )}
              <Text>
                Would you like to generate an activation link for the user?
              </Text>
            </VStack>
          </VStack>
        }
        actionBtnCaption="Yes"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={onGenerateActivationLink} // Generate link, display toast, close the modal and open another modal displaying activation link
        isOpen={isGenerateActivationLinkOpen}
        onClose={onGenerateActivationLinkClose}
      />
    );

  const ModalDisplayActivationLink = // Display activation link with only an OK button
    (
      <BasicModal
        body={
          <Formik
            enableReinitialize
            initialValues={{
              AccountActivationLink: activationLink, // get Activation link from the state
            }}
          >
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                <FormikInput
                  label="Account Activation Link"
                  name="AccountActivationLink"
                  type="readOnly"
                />
                <Alert status="info">
                  <AlertIcon />
                  Please copy the Account Activation Link and pass it to the
                  user to complete the account activation process.
                </Alert>
              </VStack>
            </Form>
          </Formik>
        }
        title="Account Activation Link"
        actionBtnCaption="Ok"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={onDisplayActivationLinkClose}
        isOpen={isDisplayActivationLinkOpen}
        onClose={onDisplayActivationLinkClose}
        closeOnOverlayClick={false} // disable closing the modal when clicked on overlay
        cancelBtnEnable={false} // hide cancel button
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
          <LinkOverlay as={Link} to={href} />
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
                  onClick={onGenerateActivationLinkOpen}
                >
                  Generate an activation link
                </Button>
              </HStack>
            )}
          </Stack>
        </Box>
      </LinkBox>
      {ModalGenerateActivationLink}
      {ModalDisplayActivationLink}
    </Center>
  );
};
