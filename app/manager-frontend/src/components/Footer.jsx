import {
  Center,
  Divider,
  Flex,
  HStack,
  Image,
  Link,
  Text,
  VStack,
} from "@chakra-ui/react";
import { Fragment } from "react";
import { Link as RouterLink } from "react-router-dom";

const isLocalUrl = (url) => url.startsWith("/");

const FooterLink = ({ children, url }) => {
  const linkProps = isLocalUrl(url)
    ? { to: url, as: RouterLink }
    : { href: url, isExternal: true };
  return <Link {...linkProps}>{children}</Link>;
};

const SmallFooter = ({ copyrightText, links }) => (
  <Flex justify="space-between" px={2}>
    {copyrightText && <Text>{copyrightText}</Text>}

    <HStack>
      {links.map((group, i) => (
        <Fragment key={i}>
          {i !== 0 && (
            <Center height="100%">
              <Divider orientation="vertical" />
            </Center>
          )}
          {Object.keys(group).map((k, i) => (
            <Fragment key={i}>
              {i !== 0 && (
                <Center height="100%">
                  <Divider orientation="vertical" />
                </Center>
              )}
              <FooterLink key={i} url={group[k]}>
                {k}
              </FooterLink>
            </Fragment>
          ))}
        </Fragment>
      ))}
    </HStack>
  </Flex>
);

const BigFooter = ({ copyrightText, links }) => (
  <VStack w="100%" bg="blue.50" align="stretch" p={4}>
    {copyrightText && (
      <Flex w="100%" justify="center">
        <Text>{copyrightText}</Text>
      </Flex>
    )}
    {/* Links */}
    <HStack justify="space-evenly">
      {links.map((group, i) => (
        <VStack key={i}>
          {Object.keys(group).map((k, i) => (
            <FooterLink key={i} url={group[k]}>
              {k}
            </FooterLink>
          ))}
        </VStack>
      ))}
    </HStack>

    {/* Logos */}
    <HStack justify="space-evenly">
      <Image
        h="71px"
        src="/assets/uon_rgb_trans.png"
        alt="University of Nottingham logo"
      />
    </HStack>
  </VStack>
);

export const Footer = ({ isSmall }) => {
  const copyrightText = `© ${new Date().getFullYear()} University of Nottingham`;

  const footerLinks = [
    {
      "About LinkLite": "/about",
    },
  ];

  return isSmall ? (
    <SmallFooter links={footerLinks} copyrightText={copyrightText} />
  ) : (
    <BigFooter links={footerLinks} copyrightText={copyrightText} />
  );
};
