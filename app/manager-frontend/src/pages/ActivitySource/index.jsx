import {
  VStack,
  Input,
  FormLabel,
  Box,
  Select,
  Button,
  Heading,
  Container,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { Form, Formik, useField } from "formik";
import { FaArrowRight } from "react-icons/fa";
import { FormikInput } from "../../components/forms/FormikInput";
import { FormikSelect } from "../../components/forms/FormikSelect";
import { useNavigate } from "react-router-dom";
import { validationSchema } from "./validation";

export const ActivitySource = ({ activitySource, action, id }) => {
  // TODO: Get this from the backend
  const typeOptions = [{ value: "RQUEST", text: "RQUEST" }];

  const [feedback, setFeedback] = useState();
  const submitText = !activitySource
    ? "Create Activity Source"
    : "Save changes";
  const headingText = !activitySource
    ? "Create a new Activity Source"
    : "Edit Activity Source";

  const navigate = useNavigate();

  const handleSubmit = async (values, actions) => {
    try {
      // convert all empty strings to null
      const payload = Object.entries(values).reduce(
        (a, [k, v]) => ({
          ...a,
          [k]: v !== "" ? v : null,
        }),
        {}
      );
      // post to the api
      await action({ values: payload, id: id }).json();

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
      window.scrollTo(0, 0);
    }
    actions.setSubmitting(false);
  };

  return (
    <Container>
      <VStack w="100%" align="stretch" p={4}>
        <Heading>{headingText}</Heading>
        <Formik
          onSubmit={handleSubmit}
          initialValues={
            activitySource
              ? {
                  DisplayName: activitySource.DisplayName,
                  Host: activitySource.Host,
                  Type: activitySource.Type,
                  ResourceId: activitySource.ResourceId,
                }
              : {
                  DisplayName: "",
                  Host: "",
                  Type: typeOptions[0].value,
                  ResourceId: "",
                }
          }
          validationSchema={validationSchema()}
        >
          {({ isSubmitting, values }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={8}>
                {feedback && (
                  <Alert status="error">
                    <AlertIcon />
                    {feedback}
                  </Alert>
                )}
                <FormikInput
                  label="Display name"
                  name={"DisplayName"}
                  type="DisplayName"
                />
                <FormikInput label="Host URl" name={"Host"} type="Host" />
                <FormikSelect
                  label="Type"
                  name={"Type"}
                  type="Type"
                  options={typeOptions}
                />
                <FormikInput
                  label="Resource Id"
                  name={"ResourceId"}
                  type="ResourceId"
                />
                <Button
                  w="full"
                  leftIcon={<FaArrowRight />}
                  colorScheme="blue"
                  type="submit"
                  disabled={isSubmitting}
                  isLoading={isSubmitting}
                >
                  {submitText}
                </Button>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
