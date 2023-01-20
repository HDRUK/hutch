import {
  VStack,
  Flex,
  Alert,
  AlertIcon,
  useDisclosure,
  Text,
  useToast,
} from "@chakra-ui/react";
import { useState, useRef } from "react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { useNavigate } from "react-router-dom";
import { BasicModal } from "components/BasicModal";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { validationSchema } from "./validation";
import { useBackendApi } from "contexts/BackendApi";

export const User = ({ data, action, id }) => {
  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState();

  const { isOpen: isRegisterUserOpen, onClose: onRegisterUserClose } =
    useDisclosure({ defaultIsOpen: true }); // Handle Register User modal

  const {
    isOpen: isDisplayLinkOpen,
    onOpen: onDisplayLinkOpen,
    onClose: onDisplayLinkClose,
  } = useDisclosure(); // Handle the modal that displays generated link

  const onCloseHandler = () => {
    onRegisterUserClose();
    setFeedback(null);
  };
  const toast = useToast();
  // toast configured for the User page
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

  const navigate = useNavigate();

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const headingText = // Manage User page heading
    (
      <Flex>
        <Text
          color={"blue.500"}
          fontWeight={600}
          letterSpacing={1.1}
          fontSize={"2xl"}
          textTransform={"uppercase"}
        >
          Register new user
        </Text>
        {data && (
          <Text pl={1} fontWeight={600} letterSpacing={1.1} fontSize={"2xl"}>
            {userName}
          </Text>
        )}
      </Flex>
    );
  const { account } = useBackendApi();
  const [generatedLink, setGeneratedLink] = useState();

  const handleSubmit = async (values, actions) => {
    // handle submission for an update or registering new user
    try {
      // post to the api
      const actionResponse = await action({
        values,
        id,
      }).json();

      displayToast({
        title: `User ${values.username} successfully registered!`,
        status: "success",
        duration: 1500,
      });
      const response = await account.generateActivationLink({
        id: actionResponse.id,
      }).json(); // generate user account activation link
     
      setGeneratedLink(response.activationLink);
      onDisplayLinkOpen();
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
      scrollTargetIntoView();
    }
    actions.setSubmitting(false);
    onRegisterUserClose();
    onCloseHandler();
  };

  const formRef = useRef();
  const ModalRegisterUser = // Modal for new User Registration
    (
      <BasicModal
        body={
          <Formik
            enableReinitialize
            innerRef={formRef}
            initialValues={{ username: "" }}
            onSubmit={handleSubmit}
            validationSchema={validationSchema()}
          >
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                {feedback && (
                  <Alert status="error">
                    <AlertIcon />
                    {feedback}
                  </Alert>
                )}
                <FormikInput label="Username" placeholder={"@username"} name="username" />
              </VStack>
            </Form>
          </Formik>
        }
        title={headingText}
        actionBtnCaption="Register"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={() => {
          formRef.current.handleSubmit();
        }}
        isOpen={isRegisterUserOpen}
        onClose={() => navigate("/home/userlist")} // navigate to userlist when modal is closed
      />
    );
  const GenerateLinkModal = (
    <BasicModal
      body={
        <Formik
          enableReinitialize
          initialValues={
            { Generate: generatedLink } // get activation reset link
          }
        >
          <Form noValidate>
            <VStack align="stretch" spacing={4}>
              <FormikInput
                label={"Link"}
                name={"Generate"}
                initialValues={generatedLink}
                type="readOnly"
              />
              <Alert status="info">
                <AlertIcon />
                Please copy this link and pass it to the user to complete the
                registration process.
              </Alert>
            </VStack>
          </Form>
        </Formik>
      }
      title={"Generate Link"}
      actionBtnCaption="Ok"
      actionBtnColorScheme="blue"
      isLoading={isLoading}
      onAction={() => navigate("/home/userlist")}
      isOpen={isDisplayLinkOpen}
      onClose={() => navigate("/home/userlist")}
      closeOnOverlayClick={false} // disable closing the modal when clicked on overlay
      cancelBtnEnable={false} // display cancel if error else hide cancel button
    />
  );

  return (
    <>
      {ModalRegisterUser}
      {GenerateLinkModal}
    </>
  );
};
