import React from "react";
import {
  Flex,
  HStack,
  Link,
  Text,
  useColorModeValue,
  VStack,
} from "@chakra-ui/react";
import HdrLogo from "@site/static/img/hdruk_logo.svg";

export const Features = () => (
  <HStack p={5} justify="center" align="start" spacing={150}>
    <OpenSource />
    <Secure />
    <Hdr />
  </HStack>
);

const Feature = ({ graphic, children }) => (
  <VStack w="10em" textAlign="center">
    <Flex align="center" justify="center" width="100%" height="130px">
      {graphic}
    </Flex>

    <Text>{children}</Text>
  </VStack>
);

const OpenSource = () => (
  <Feature graphic={<Text fontSize={72}>❤️</Text>}>
    Hutch is Open Source software under the permissive MIT license.
  </Feature>
);

const Hdr = () => {
  return (
    <Feature
      graphic={
        <Link href="https://www.hdruk.ac.uk/" isExternal title="HDR UK">
          <HdrLogo role="img" />
        </Link>
      }
    >
      Hutch is being developed for use in HDR UK's{" "}
      <Link
        href="https://www.healthdatagateway.org/about/cohort-discovery"
        isExternal
      >
        Cohort Discovery
      </Link>{" "}
      toolset.
    </Feature>
  );
};

const Secure = () => {
  const { green, yellow } = useColorModeValue(
    { green: "green.400", yellow: "yellow.500" },
    { green: "green.300", yellow: "yellow.200" }
  );

  return (
    <Feature graphic={<Text fontSize={72}>🔒</Text>}>
      Make your data discoverable{" "}
      <Text as="span" color={green} fontWeight="bold">
        safely
      </Text>{" "}
      and{" "}
      <Text as="span" color={yellow} fontWeight="bold">
        securely
      </Text>
      , without directly sharing it.
    </Feature>
  );
};
