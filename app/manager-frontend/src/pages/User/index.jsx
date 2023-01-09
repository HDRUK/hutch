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
import { FormikInput } from "../../components/forms/FormikInput";
import { useNavigate } from "react-router-dom";
import { BasicModal } from "components/BasicModal";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { validationSchema } from "./validation";

export const User = ({ data, action, id }) => {
  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState();

  const { isOpen: isRegisterUserOpen, onClose: onRegisterUserClose } =
    useDisclosure({ defaultIsOpen: true }); // Handle Register User modal

  const onCloseHandler = () => {
    onClose();
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

  const handleSubmit = async (values, actions) => {
    // handle submission for an update or registering new user
    try {
      values.username = "@" + values.username;
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
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
      scrollTargetIntoView();
    }
    actions.setSubmitting(false);
    onRegisterUserClose();
    navigate("/home/userlist");
    onCloseHandler();
  };

  const formRef = useRef();
  const ModalRegisterUser = // Modal for displaying New User Registration
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
                <FormikInput label="Username" name="username" />
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
        onClose={() => navigate("/home/userlist")} // navigate back to userlist when modal is closed
      />
    );

  return <>{ModalRegisterUser}</>;
};
