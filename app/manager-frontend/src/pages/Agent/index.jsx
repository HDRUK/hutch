import {
  VStack,
  Flex,
  Button,
  Container,
  Alert,
  AlertIcon,
  useDisclosure,
  HStack,
  Text,
  useToast,
} from "@chakra-ui/react";
import { useState, useRef } from "react";
import { Form, Formik } from "formik";
import { FaTrash, FaTimes, FaRegSave, FaEyeSlash } from "react-icons/fa";
import { FormikInput } from "../../components/forms/FormikInput";
import { useNavigate } from "react-router-dom";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";
import { BasicModal } from "components/BasicModal";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { validationSchema } from "./validation";

export const Agent = ({ data, action, id }) => {
  const [agentId, setAgentId] = useState(data?.name);
  const [agentName, setAgentName] = useState(data?.name);
  const [agentClientId, setAgentClientId] = useState(data?.clientId);
  const [agentClientSecret, setAgentClientSecret] = useState(
    data?.clientSecret
  );
  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState();

  const {
    isOpen: isDeleteOpen,
    onOpen: onDeleteOpen,
    onClose: onDeleteClose,
  } = useDisclosure(); // Handle Delete modal

  const {
    isOpen: isGenerateSecretOpen,
    onOpen: onGenerateSecretOpen,
    onClose: onGenerateSecretClose,
  } = useDisclosure(); // Handle Generate new secret modal

  const {
    isOpen: isDisplayRegisteredAgentOpen,
    onOpen: onDisplayRegisteredAgentOpen,
    onClose: onDisplayRegisteredAgentClose,
  } = useDisclosure(); // Handle Display Registered Agent modal

  const { isOpen: isRegisterAgentOpen, onClose: onRegisterAgentClose } =
    useDisclosure({ defaultIsOpen: true }); // Handle Register Agent modal

  const toast = useToast();
  // toast configured for the Agent page
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

  const { agent } = useBackendApi();

  const isUpdate = !data ? false : true; // check if editing or registering new agent

  const submitText = !isUpdate ? "Register" : "Save";

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const headingText = // Manage Agent page heading
    (
      <Flex>
        <Text
          color={"blue.500"}
          fontWeight={600}
          letterSpacing={1.1}
          fontSize={"2xl"}
          textTransform={"uppercase"}
        >
          {!data ? "Register an Agent" : "Manage an Agent:"}{" "}
        </Text>
        {data && (
          <Text pl={1} fontWeight={600} letterSpacing={1.1} fontSize={"2xl"}>
            {agentName}
          </Text>
        )}
      </Flex>
    );

  const onDeleteAgent = async () => {
    // handle delete action within the Agent page
    setIsLoading(true);
    await agent.delete({ id: id });
    setIsLoading(false);
    onDeleteClose();
    // redirect with a toast
    navigate("/home/agentlist");
    displayToast({
      title: "Agent successfully deleted!",
      status: "success",
      duration: 1500,
    });
  };

  const handleSubmit = async (values, actions) => {
    // handle submission for an update or registering new agent
    try {
      // post to the api
      const actionResponse = await action({
        values,
        id,
      }).json();

      displayToast({
        title: `Agent ${values.Name} successfully ${
          !isUpdate ? "registered!" : "updated"
        }`,
        status: "success",
        duration: 1500,
      });

      if (!isUpdate) {
        // Update state if registering a new agent
        setAgentId(actionResponse?.id);
        setAgentName(actionResponse?.name);
        setAgentClientId(actionResponse?.clientId);
        setAgentClientSecret(actionResponse?.clientSecret);

        onRegisterAgentClose(); // close the register modal
        onDisplayRegisteredAgentOpen(); // display the newly registerd agent in a separate modal
      }

      if (isUpdate) navigate("/home/agentlist"); // redirect to agentlist if updating an existing agent
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
      scrollTargetIntoView();
    }
    actions.setSubmitting(false);
  };

  const onGenerateNewSecret = async () => {
    // handle submission for new agent secret
    setIsLoading(true);
    const actionResponse = await agent.updateAgentSecret({ id: id }).json(); // generate and update the agent secret
    if (actionResponse) {
      setAgentClientSecret(actionResponse.clientSecret); // update the state
    }
    setIsLoading(false);
    onGenerateSecretClose();
    displayToast({
      title: "New client secret generted",
      status: "success",
      duration: 900,
    });
  };

  const ModalGenerateNewSecret = // Modal for displaying Generate Agent Secret
    (
      <BasicModal
        body={
          <VStack>
            <VStack>
              <Text>
                <Alert status="info">
                  <AlertIcon />
                  Generating a new Client Secret will override your exisiting
                  Client Secret.
                </Alert>
              </Text>
              <Text fontWeight="bold">
                Would you like to generate a new Client Secret ?
              </Text>
            </VStack>
          </VStack>
        }
        actionBtnCaption="Yes"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={onGenerateNewSecret}
        isOpen={isGenerateSecretOpen}
        onClose={onGenerateSecretClose}
      />
    );

  const ModalDeleteAgent = // Modal for displaying Delete Agent action
    (
      <DeleteModal
        title={`Delete an Agent?`}
        body={
          <VStack>
            <VStack>
              <Text>Are you sure you want to delete this Agent:</Text>
              <Text fontWeight="bold">{agentName}</Text>
            </VStack>
          </VStack>
        }
        isLoading={isLoading}
        isOpen={isDeleteOpen}
        onClose={onDeleteClose}
        onDelete={onDeleteAgent}
      />
    );

  const formRef = useRef();
  const ModalRegisterAgent = // Modal for displaying New Agent Registration
    (
      <BasicModal
        body={
          <Formik
            enableReinitialize
            innerRef={formRef}
            initialValues={{ Name: "" }}
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
                <FormikInput label="Name" name="Name" />
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
        isOpen={isRegisterAgentOpen}
        onClose={() => navigate("/home/agentlist")} // load list if modal closed
      />
    );

  const ModalDisplayRegisteredAgent = // Display newly created agent information with only an OK button
    (
      <BasicModal
        body={
          <Formik
            enableReinitialize
            initialValues={{
              Name: agentName,
              ClientId: agentClientId,
              ClientSecret: agentClientSecret,
            }}
          >
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                <FormikInput
                  label="Client Id"
                  name="ClientId"
                  type="readOnly"
                />
                <FormikInput
                  label="Client Secret"
                  name="ClientSecret"
                  type="readOnly"
                />
                <Alert status="info">
                  <AlertIcon />
                  Please make a note of the Client Secret as it is only
                  displayed once at the time of the Agent registration or after
                  you generate a new Client secret
                </Alert>
              </VStack>
            </Form>
          </Formik>
        }
        title={agentName}
        actionBtnCaption="Ok"
        actionBtnColorScheme="blue"
        isLoading={isLoading}
        onAction={() => navigate(`/agents/${agentId}`)} // load agent page when user clicks on Ok button
        isOpen={isDisplayRegisteredAgentOpen}
        onClose={onDisplayRegisteredAgentClose}
        closeOnOverlayClick={false} // disable closing the modal when clicked on overlay
        cancelBtnEnable={false} // hide cancel button
      />
    );

  const NewRegistrationModals = (
    <>
      {ModalRegisterAgent}
      {ModalDisplayRegisteredAgent}
    </>
  );

  if (!data) return <>{!isUpdate ? NewRegistrationModals : null}</>; // only load NewRegistrationModals if registering an agent

  return (
    // else load Agent edit page
    <Container my={8} ref={scrollTarget}>
      <VStack align="stretch" spacing={4} p={4} pb={10}>
        <Flex justify="space-between">
          {headingText}
          <Button
            leftIcon={<FaTimes />}
            variant="outline"
            colorScheme="gray"
            onClick={() => navigate("/home/agentlist")}
          >
            Cancel
          </Button>
        </Flex>

        <Formik
          enableReinitialize
          onSubmit={handleSubmit}
          initialValues={{
            Name: agentName,
            ClientId: agentClientId,
            ClientSecret: agentClientSecret,
          }}
          validationSchema={validationSchema()}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                {feedback && (
                  <Alert status="error">
                    <AlertIcon />
                    {feedback}
                  </Alert>
                )}
                <FormikInput label="Name" name="Name" />
                <FormikInput
                  label="Client Id"
                  name="ClientId"
                  type="readOnly"
                />
                <FormikInput
                  label="Client Secret"
                  name="ClientSecret"
                  type="readOnly"
                  placeholder={
                    !agentClientSecret && "***********************************"
                  }
                />

                <Alert status="info">
                  <AlertIcon />
                  Your Client Secret is only displayed once at the time of an
                  Agent registration or after you generate a new Client secret
                </Alert>

                <HStack justifyContent="end">
                  <Button
                    pl="25px"
                    pr="25px"
                    leftIcon={<FaEyeSlash />}
                    colorScheme="green"
                    onClick={onGenerateSecretOpen}
                  >
                    Generate a new Client Secret
                  </Button>
                </HStack>
                <HStack justify={"space-between"}>
                  <Button
                    leftIcon={<FaRegSave />}
                    colorScheme="blue"
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {submitText}
                  </Button>
                  <Button
                    leftIcon={<FaTrash />}
                    variant="outline"
                    colorScheme="red"
                    onClick={onDeleteOpen}
                  >
                    Delete
                  </Button>
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
      {ModalGenerateNewSecret}
      {ModalDeleteAgent}
    </Container>
  );
};
