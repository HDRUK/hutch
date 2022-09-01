import {
  VStack,
  Flex,
  Button,
  Heading,
  Container,
  Alert,
  AlertIcon,
  useDisclosure,
  HStack
} from "@chakra-ui/react";
import { useState } from "react";
import { Form, Formik } from "formik";
import { FaArrowRight, FaTrash } from "react-icons/fa";
import { FormikInput } from "../../components/forms/FormikInput";
import { FormikSelect } from "../../components/forms/FormikSelect";
import { useNavigate } from "react-router-dom";
import { validationSchema } from "./validation";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";
import { useActivitySourceResultsModifiersList } from "api/activitysources"
import { useDataSourceList } from "api/datasource";
import { getDateDaysAgo } from "helpers/dates";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { ResultsModifiers } from "components/resultsmodifiers/ResultsModifiers";

export const ActivitySource = ({ activitySource, action, id }) => {
  // TODO: Get this from the backend
  const typeOptions = [{ id: "RQUEST" }];
  const { data: datasourceOptions } = useDataSourceList();
  const { data: modifiers } = useActivitySourceResultsModifiersList(id);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const { activitysource } = useBackendApi();
  const [feedback, setFeedback] = useState();
  const submitText = !activitySource
    ? "Create Activity Source"
    : "Save changes";
  const headingText = !activitySource
    ? "Create a new Activity Source"
    : "Edit Activity Source";

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const navigate = useNavigate();
  const onDeleteSource = async () => {
    await activitysource.delete({ id: id });
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
    <Container my={8} ref={scrollTarget}>
      <VStack w="100%" align="stretch" spacing={4}>
        <Flex justify="space-between">
          <Heading>{headingText}</Heading>
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
                <HStack>
                  <Button
                    w="200px"
                    leftIcon={<FaArrowRight />}
                    colorScheme="blue"
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {submitText}
                  </Button>
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
      <ResultsModifiers modifiers={modifiers}></ResultsModifiers>
      <DeleteModal
        title={`Delete Activity Source ${id}`}
        body="Are you sure you want to delete this activity source? You will not be able to reverse this"
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
      />
    </Container>
  );
};
