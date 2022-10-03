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
} from "@chakra-ui/react";
import { useState } from "react";
import { Form, Formik } from "formik";
import { FaArrowRight, FaTrash, FaTimes } from "react-icons/fa";
import { FormikInput } from "../../components/forms/FormikInput";
import { FormikSelect } from "../../components/forms/FormikSelect";
import { useNavigate } from "react-router-dom";
import { validationSchema } from "./validation";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";
import { useDataSourceList } from "api/datasource";
import { getDateDaysAgo } from "helpers/dates";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { ResultsModifiers } from "components/resultsmodifiers/ResultsModifiers";

export const ActivitySource = ({ activitySource, action, id }) => {
  const typeOptions = [{ id: "RQUEST" }];
  const { data: datasourceOptions } = useDataSourceList();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const { activitysource } = useBackendApi();
  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState();
  const submitText = !activitySource ? "Create" : "Save";
  const headingText = !activitySource ? (
    <Flex>
      <Text
        color={"blue.500"}
        fontWeight={600}
        letterSpacing={1.1}
        fontSize={"2xl"}
        textTransform={"uppercase"}
      >
        Create Activity Source
      </Text>
    </Flex>
  ) : (
    <Flex>
      <Text
        color={"blue.500"}
        fontWeight={600}
        letterSpacing={1.1}
        fontSize={"2xl"}
        textTransform={"uppercase"}
      >
        Editing:
      </Text>
      <Text pl={1} fontWeight={600} letterSpacing={1.1} fontSize={"2xl"}>
        {activitySource.displayName}
      </Text>
    </Flex>
  );
  const isUpdate = !activitySource ? false : true;
  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const navigate = useNavigate();
  const onDeleteSource = async () => {
    setIsLoading(true);
    await activitysource.delete({ id: id });
    setIsLoading(false);
    onClose();
    // redirect with a toast
    navigate("/", {
      state: {
        toast: {
          title: "Activity Source successfully deleted!",
          status: "success",
          duration: 2500,
          isClosable: true,
        },
      },
    });
  };
  const handleSubmit = async (values, actions) => {
    try {
      // post to the api
      await action({
        values,
        id,
      }).json();

      // redirect with a toast
      navigate("/", {
        state: {
          toast: {
            title: "Activity source successfully submitted!",
            status: "success",
            duration: 2500,
            isClosable: true,
          },
        },
      });
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
      scrollTargetIntoView();
    }
    actions.setSubmitting(false);
  };

  return (
    <Container my={8} ref={scrollTarget} style={{ maxWidth: "850px" }}>
      <VStack align="stretch" spacing={4} p={4} pb={10}>
        <Flex justify="space-between">
          {headingText}
          <Button
            leftIcon={<FaTimes />}
            variant="outline"
            colorScheme="red"
            onClick={() => navigate("/home")}
          >
            Cancel
          </Button>
        </Flex>

        <Formik
          onSubmit={handleSubmit}
          initialValues={
            activitySource
              ? {
                  DisplayName: activitySource.displayName,
                  Host: activitySource.host,
                  Type: activitySource.type,
                  ResourceId: activitySource.resourceId,
                  TargetDataSource:
                    datasourceOptions.find(
                      (item) => item.id === activitySource.targetDataSource
                    )?.id ?? "",
                }
              : {
                  DisplayName: "",
                  Host: "",
                  Type: typeOptions[0].id,
                  ResourceId: "",
                  TargetDataSource: "",
                }
          }
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
                <FormikInput label="Display name" name="DisplayName" />
                <FormikInput label="Host URl" name={"Host"} type="Host" />
                <FormikSelect
                  label="Type"
                  name={"Type"}
                  options={typeOptions.map((item) => ({
                    value: item.id,
                    label: item.id,
                  }))}
                />
                <FormikInput label="Resource Id" name={"ResourceId"} />
                <FormikSelect
                  label="Target Data Source"
                  name="TargetDataSource"
                  options={datasourceOptions.map((item) => ({
                    value: item.id,
                    label:
                      new Date(item.lastCheckin) > getDateDaysAgo(2)
                        ? item.id
                        : `${item.id} (Inactive)`,
                  }))}
                  alert={
                    datasourceOptions.length == 0 && {
                      status: "error",
                      message: `There are no Data Sources registered. Please "check in" an Agent with this Manager.`,
                    }
                  }
                  hasEmptyDefault
                />
                {isUpdate ? null : (
                  <Alert status="info">
                    <AlertIcon />
                    Result Modifiers can be added after an Activity Source is
                    created.
                  </Alert>
                )}
                <HStack justify={"space-between"}>
                  <Button
                    leftIcon={<FaArrowRight />}
                    colorScheme="blue"
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {submitText}
                  </Button>
                  {id && (
                    <Button
                      leftIcon={<FaTrash />}
                      variant="outline"
                      colorScheme="red"
                      onClick={onOpen}
                    >
                      Delete
                    </Button>
                  )}
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
      {isUpdate ? (
        <>
          <ResultsModifiers id={id}></ResultsModifiers>
          <DeleteModal
            title={`Delete Activity Source?`}
            body={
              <VStack>
                <Text>
                  Are you sure you want to delete this activity source:
                </Text>
                <Text fontWeight="bold">{activitySource.displayName}</Text>
                <Text>You will not be able to reverse this!</Text>
              </VStack>
            }
            isLoading={isLoading}
            isOpen={isOpen}
            onClose={onClose}
            onDelete={onDeleteSource}
          />
        </>
      ) : null}
    </Container>
  );
};
