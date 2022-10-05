import React from "react";
import Layout from "@theme/Layout";
import { Heading, Text, SimpleGrid, VStack, HStack } from "@chakra-ui/react";
import { FaBook, FaDownload } from "react-icons/fa";
import { Features } from "@site/src/components/homepage/Features";
import LinkButton from "../components/LinkButton";

const HeroButton = (p) => (
  <LinkButton
    size="xl"
    boxShadow="2xl"
    _hover={{ boxShadow: "dark-lg", top: "-5px" }}
    transition="all ease .2s"
    top="0"
    {...p}
  >
    {p.children || null}
  </LinkButton>
);

const HeroBanner = () => {
  const responsiveChildWidths = { sm: "100%", md: "70%" };

  const randColor = () =>
    ["orange", "purple", "teal", "cyan", "pink", "blue"][
      Math.floor(Math.random() * 6)
    ];

  return (
    <VStack
      bgGradient="radial(circle 1000px at top left, cyan.600, blue.900)"
      py={10}
      spacing={8}
      alignItems="center"
      color="white"
      textShadow="2px 2px 2px rgba(0,0,0,.8)"
    >
      <Heading size="4xl">
        <Text>📤🐇 Hutch</Text>
      </Heading>

      <Heading
        size="2xl"
        p={5}
        width={responsiveChildWidths}
        textAlign="center"
        color={`${randColor()}.200`}
      >
        <Text as="span">Federated Data Discovery</Text>
      </Heading>

      <HStack spacing={16}>
        <HeroButton
          colorScheme="green"
          leftIcon={<FaDownload />}
          href="https://github.com/hdruk/hutch/releases"
        >
          Download now
        </HeroButton>

        <HeroButton
          colorScheme="blue"
          leftIcon={<FaBook />}
          to="docs/users/getting-started/installation"
        >
          Installation Guide
        </HeroButton>
      </HStack>
    </VStack>
  );
};

const LinkCardSection = () => (
  <VStack p={5} w="100%" align="stretch">
    <Heading fontWeight="medium">📚 Documentation</Heading>
    <SimpleGrid p={8} columns={{ sm: 1, md: 2 }} spacing={8}>
      {/* <DirectoryLinkCard />
      <ApiLinkCard />
      <InstallationLinkCard />
      <DeveloperLinkCard /> */}
    </SimpleGrid>
  </VStack>
);

const Index = () => {
  return (
    <Layout>
      <VStack mb={5} align="stretch">
        <HeroBanner />

        <Features />

        {/* <LinkCardSection /> */}
      </VStack>
    </Layout>
  );
};

export default Index;
