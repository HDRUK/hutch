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

import React, { useState } from "react";
import { Form, Formik, useField } from "formik";
import { FaArrowRight } from "react-icons/fa";
import { FormikInput } from "./forms/FormikInput";
import { FormikSelect } from "./forms/FormikSelect";

function LowNumberSuppressionParameters({ type }) {
  // Todo
  switch (type) {
    case "1":
      return <div>LowNumberSuppressionParameters</div>;
    default:
      return <div>LowNumberSuppressionParameters</div>;
  }
}

export default LowNumberSuppressionParameters;
