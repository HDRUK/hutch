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
} from "@chakra-ui/react";

import React, { useState, useEffect } from "react";
import { Form, Formik, useFormikContext } from "formik";
import { FaArrowRight } from "react-icons/fa";
import { FormikInput } from "./forms/FormikInput";
import { FormikSelect } from "./forms/FormikSelect";

function LowNumberSuppressionParameters({ type }) {
  const formikProps = useFormikContext();
  useEffect(() => {
    formikProps.setFieldValue("Parameters", { Threshold: null });
  }, [type]);
  // Todo: get the value of the inputs to update when the formik values update
  switch (type) {
    case "Type1":
      return (
        <>
          <FormikInput
            label="Threshold"
            name={"Parameters.Threshold"}
            type="number"
          />
        </>
      );
    default:
      return (
        <>
          <FormikInput
            label="Threshold"
            name={"Parameters.Threshold"}
            type="number"
          />
        </>
      );
  }
}

export default LowNumberSuppressionParameters;
