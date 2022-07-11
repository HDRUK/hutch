import {
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  VStack,
  Alert,
  AlertIcon,
  useDisclosure,
} from "@chakra-ui/react";

import React, { useState } from "react";
import { Form, Formik, useField } from "formik";
import { FaArrowRight } from "react-icons/fa";
import { FormikInput } from "./forms/FormikInput";
import { FormikSelect } from "./forms/FormikSelect";
import LowNumberSuppressionParameters from "./LowNumberSuppressionParameters";
import { validationSchema } from "pages/ResultsModifier/validation";

export const ConfigureResultsModifierModal = ({
  initialData,
  isOpen,
  onClose,
  action,
}) => {
  const {
    isOpen: isConfirmOpen,
    onOpen: onConfirmOpen,
    onClose: onConfirmClose,
  } = useDisclosure();
  const [feedback, setFeedback] = useState();

  // Todo: Get this from backend
  const typeOptions = [
    { value: "Type1", text: "Type1" },
    { value: "Type2", text: "Type2" },
  ];

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
      await action({ values: payload }).json();
      onClose();
    } catch (e) {
      console.error(e);
      onConfirmClose();
      setFeedback("Something went wrong!");
      window.scrollTo(0, 0);
    }
    actions.setSubmitting(false);
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>
          {initialData ? "Update Results Modifier" : "Create Results Modifier"}
        </ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Formik
            onSubmit={handleSubmit}
            initialValues={
              initialData
                ? {
                    Order: initialData.order,
                    Type: initialData.type,
                    // capitalise the object keys in the parameters object
                    Parameters: Object.fromEntries(
                      Object.entries(initialData.parameters).map(([k, v]) => [
                        k.charAt(0).toUpperCase() + k.slice(1),
                        v,
                      ])
                    ),
                  }
                : {
                    Order: "",
                    Type: typeOptions[0].value,
                    Parameters: {},
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
                  <FormikInput label="Order" name={"Order"} type="Order" />
                  <FormikSelect
                    label="Type"
                    name={"Type"}
                    type="Type"
                    options={typeOptions}
                  />
                  <LowNumberSuppressionParameters type={values.Type} />
                  <Button
                    w="full"
                    leftIcon={<FaArrowRight />}
                    colorScheme="blue"
                    onClick={onConfirmOpen}
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {initialData
                      ? "Update Results Modifier"
                      : "Create Results Modifier"}
                  </Button>
                  <Modal isOpen={isConfirmOpen} onClose={onConfirmClose}>
                    <ModalOverlay />
                    <ModalContent>
                      <ModalHeader>
                        {initialData
                          ? "Update Results Modifier"
                          : "Create Results Modifier"}
                      </ModalHeader>
                      <ModalCloseButton />
                      <Form noValidate>
                        <ModalBody>Are you sure you want to do this?</ModalBody>
                        <ModalFooter>
                          <Button
                            variant="ghost"
                            mr={3}
                            onClick={onConfirmClose}
                          >
                            Close
                          </Button>
                          <Button colorScheme="blue" type="submit">
                            Confirm
                          </Button>
                        </ModalFooter>
                      </Form>
                    </ModalContent>
                  </Modal>
                </VStack>
              </Form>
            )}
          </Formik>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
};
